import { inject } from '@angular/core';
import { ResolveFn, Router } from '@angular/router';
import { MemberService } from '../../core/services/member-service';
import { Member } from '../../types/member';
import { EMPTY } from 'rxjs';

// resolver to get member data before navigating to member detailed component
// in angular, resolver used to pre-fetch data before navigating to a route
// so that the component has the data it needs when it is initialized
export const memberResolver: ResolveFn<Member> = (route, state) => {
  const memberService = inject(MemberService);
  const router = inject(Router);
  const id = route.paramMap.get('id');
  if(!id) {
    router.navigate(['/not-found']);
    // we return EMPTY observable to indicate that the resolution failed
    // we mustn't return null or undefined here, because the resolver expects an observable
    return EMPTY;
  }
  return memberService.getMember(id);
};
