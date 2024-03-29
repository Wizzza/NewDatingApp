using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public UsersController (IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());
            userParams.CurrentUserName = User.GetUserName();

            if(string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = user.Gender == "male" ? "female" : "male";

            var users = await _userRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize,users.TotalCount, users.TotalPages);

            return Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult<PhotoDto>> UpdateMember(MemberUpdateDto memberUpdateDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            _mapper.Map(memberUpdateDto, user);
            _userRepository.update(user);

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("failed to update");
        }
        [HttpPost("add-photo")]
        public  async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());
            var result = await _photoService.AddPhotoAsync(file);

            if(result.Error != null) 
                return BadRequest(result.Error.Message);
            
            var photo = new Photo {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(user.Photos.Count == 0 )
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);
            if (await _userRepository.SaveAllAsync())
            {
                // return _mapper.Map<PhotoDto>(photo);
                return CreatedAtRoute("GetUser", new {username = user.UserName}, _mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("problem adding Photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if(photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if(currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            var photoToDelete = user.Photos.FirstOrDefault(x => x.Id == photoId);


            if(photoToDelete == null)
                return NotFound();
            if(photoToDelete.IsMain) 
                return BadRequest("Please choose a different mainphoto before deleting it");
            if(photoToDelete.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photoToDelete.PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photoToDelete);
            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("failed to delete the photo");
        }
    }
}