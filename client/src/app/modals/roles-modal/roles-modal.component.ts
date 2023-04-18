import { AdminService } from './../../_services/admin.service';
import { AccountService } from './../../_services/account.service';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit {
  username = '';
  availableRoles: any[] = [];
  selectedRoles: string[] = [];


  constructor(public bsModalRef: BsModalRef, private accountService: AccountService) { }

  ngOnInit(): void {
  }

  updateChecked(checkedValue: string){
    const index =this.selectedRoles.indexOf(checkedValue);
    index !== -1 ? this.selectedRoles.splice(index, 1) : this.selectedRoles.push(checkedValue);
  }
}
