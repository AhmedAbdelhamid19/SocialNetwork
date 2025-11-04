import { Component, inject, OnInit, Signal, signal } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { Photo } from '../../../types/member';
import { ImageUpload } from "../../../shared/image-upload/image-upload";
import { AccountService } from '../../../core/services/account-service';

@Component({
  selector: 'app-member-photos',
  imports: [ImageUpload],
  templateUrl: './member-photos.html',
  styleUrl: './member-photos.css'
})
export class MemberPhotos implements OnInit {
  protected memberService = inject(MemberService);
  protected accountService = inject(AccountService);
  private route = inject(ActivatedRoute);
  protected photos = signal<Photo[]>([]);
  protected loading = signal(false);
  
  ngOnInit(): void {
    // instead of setting up your subscription in the constructor, you should use ngOnInit:
    // ngOnInit is the standard Angular lifecycle hook for performing side effects like HTTP calls.
    // The constructor should only handle dependency injection, not logic.
    //When going out to get data from an API, it's typically done in the ng on init.
    const memberId = this.route.parent?.snapshot.paramMap.get('id')!;
    if(memberId) {
      this.memberService.getMemberPhotos(memberId).subscribe({
        next: (photos) => {
          let photo = photos.find(p => p.url === this.memberService.member()?.imageUrl);
          if(photo) {
            let reordered = [photo, ...photos.filter(p => p.id !== photo!.id)];
            this.photos.set(reordered)
          }
        }
      });
    }
  }
  onUploadImage(file: File) {
    this.loading.set(true);
    this.memberService.uploadPhoto(file).subscribe({
      next: photo => {
        this.memberService.editMode.set(false);
        this.loading.set(false); 
        this.photos.set([...this.photos(), photo]);
      },
      error: err => {
        console.error('Error uploading photo:', err);
        this.loading.set(false);
      }
    });
  }
  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo.id).subscribe({
      next: () => {
        const currentUser = this.accountService.currentUser();
        const member = this.memberService.member();
        if(currentUser) {
          const updatedUser = {...currentUser, imageUrl: photo.url};
          this.accountService.setCurrentUser(updatedUser);
        }
        if(member) {
          const updatedMember = {...member, imageUrl: photo.url};
          this.memberService.member.set(updatedMember);
        }
        
        // Reorder photos to make selected photo first
        const currentPhotos = this.photos();
        const reorderedPhotos = [
          photo,
          ...currentPhotos.filter(p => p.id !== photo.id)
        ];
        this.photos.set(reorderedPhotos);
      },
      error: err => {
        console.error('Error setting main photo:', err);
      }
    });
  }
  deletePhoto(photo: Photo) {
    this.memberService.deletePhoto(photo.id).subscribe({
      next: () => {
        this.photos.set(this.photos().filter(p => p.id !== photo.id));
      },
      error: err => {
        console.error('Error deleting photo:', err);
        // Optionally, provide user feedback here (e.g., show a toast or alert)
      }
    });
  }
}