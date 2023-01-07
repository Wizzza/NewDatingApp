import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/models/member';
import { Pagination } from 'src/app/models/pagination';
import { user } from 'src/app/models/user';
import { userParams } from 'src/app/models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  //members$: Observable<Member[]>;
  members: Member[];
  pagination: Pagination | undefined;
  userParams: userParams | undefined;
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Female'}]

  constructor(private memberService: MembersService) {
      this.userParams = this.memberService.getUserParams();
   }

  ngOnInit(): void {
    this.loadMembers();
  }

  resetFilters(){
      this.userParams = this.memberService.resetUserParams();
      this.loadMembers();
  }

  loadMembers() {
    if(this.userParams){
      this.memberService.setUserparams(this.userParams);
       this.memberService.getMembers(this.userParams).subscribe({
        next: Response => {
          if(Response.result && Response.pagination) {
        this.members = Response.result;
        this.pagination = Response.pagination;
        }
    }
  })
  }
}

  pageChanged(event: any){
    if(this.userParams && this.userParams.pageNumber !== event.page){
      this.userParams.pageNumber = event.page;
      this.memberService.setUserparams(this.userParams);
      this.loadMembers();
    }
  }

}
