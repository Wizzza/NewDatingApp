import { user } from './../../models/user';
import { AdminService } from './../../_services/admin.service';
import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';
import { initialState } from 'ngx-bootstrap/timepicker/reducer/timepicker.reducer';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: user[] = [];
  bsModalRef: BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>();
  availableRoles = [
    'Admin', 
    'Moderator', 
    'Member'
  ];

  constructor(private AdminService: AdminService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles(){
    this.AdminService.getUsersWithRoles().subscribe({
      next: users => this.users = users
    })
  }

  openRolesModal(user: user) {
 ///  const inintialState: ModalOptions ={
 ///    initialState:{
 ///      username: user.username,
 ///      availableRoles: this.availableRoles,
 ///      selectedRoles: user.roles,
 ///      title: 'Test modal'
 ///    }
 ///  }
    const config = {
      class: 'modal-dialog-centered',
      initialState:{
             username: user.username,
             availableRoles: this.availableRoles,
             selectedRoles: [...user.roles],
             title: 'Test modal'
      }
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    this.bsModalRef.onHide.subscribe({
      next: () => {
        const selectedRoles = this.bsModalRef.content.selectedRoles;
        if(!this.arrayEqual(selectedRoles!, user.roles))
          this.AdminService.updateUserRoles(user.username, selectedRoles).subscribe({
            next: roles => user.roles = roles
          })
      }
    })
  }

  private arrayEqual(arr1: any[], arr2: any[])
  {
    return JSON.stringify(arr1.sort()) === JSON.stringify(arr2.sort());
  }

}
