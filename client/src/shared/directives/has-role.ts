import { Directive, inject, input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../../core/services/account-service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRole implements OnInit {
  appHasRole = input<string[]>([]);
  private accountService = inject(AccountService);
  private viewContainer = inject(ViewContainerRef);
  private templateRef = inject(TemplateRef);

  ngOnInit(): void {
      if (this.accountService.currentUser()?.roles.some(r => this.appHasRole().includes(r))) {
        this.viewContainer.createEmbeddedView(this.templateRef);
      } else {
        this.viewContainer.clear();
      }
  }
}