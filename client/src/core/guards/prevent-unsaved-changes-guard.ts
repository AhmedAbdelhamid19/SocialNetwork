import { CanDeactivateFn } from '@angular/router';
import { MemberProfile } from '../../features/members/member-profile/member-profile';

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberProfile> = (component, currentRoute, currentState, nextState) => {
  if(component.editForm?.dirty) {
    return confirm('Are you sure you want to coninue? all unsaved changeds will be lost')
  }

  return true;
};
