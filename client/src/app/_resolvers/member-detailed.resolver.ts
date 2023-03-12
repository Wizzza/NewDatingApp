import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { Member } from '../models/member';
import { MembersService } from '../_services/members.service';

@Injectable({
  providedIn: 'root'
})
export class MemberDetailedResolver implements Resolve<Member> {
  constructor(private memberSerivce: MembersService) {}
  resolve(route: ActivatedRouteSnapshot): Observable<Member> {
    return this.memberSerivce.getMember(route.paramMap.get('username')!)
  }
}
