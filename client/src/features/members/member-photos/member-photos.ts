import { Component, inject, OnInit, Signal, signal } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { PhotoService } from '../../../core/services/photo-service';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { Photo } from '../../../types/member';
import { ImageUpload } from "../../../shared/image-upload/image-upload";
import { AccountService } from '../../../core/services/account-service';
import { ToastService } from '../../../core/services/toast-service';

@Component({
  selector: 'app-member-photos',
  imports: [ImageUpload],
  templateUrl: './member-photos.html',
  styleUrl: './member-photos.css'
})
export class MemberPhotos implements OnInit {
  protected memberService = inject(MemberService);
  protected photoService = inject(PhotoService);
  protected accountService = inject(AccountService);
  protected toastService = inject(ToastService);
  private route = inject(ActivatedRoute);
  protected photos = signal<Photo[]>([]);
  protected loading = signal(false);
  
  ngOnInit(): void {
    // get photo of the member and reorder to have main photo first
    const memberId = this.route.parent?.snapshot.paramMap.get('id')!;
    if(memberId) {
      console.log('Loading photos for member ID:', memberId);
      this.photoService.getMemberPhotos(memberId).subscribe({
        next: (photos: Photo[]) => {
          console.log('Photos loaded:', photos);
          const mainPhotoUrl = this.accountService.currentUser()?.imageUrl;
          if(mainPhotoUrl) {
            const photo = photos.find(p => p.url === mainPhotoUrl);
            if(photo) {
              let reordered = [photo, ...photos.filter(p => p.id !== photo!.id)];
              this.photos.set(reordered)
            }
          } else {
            this.photos.set(photos);
          }
        },
        error: (err: any) => {
          this.toastService.error('Failed to load photos.');
          console.error('Error loading photos:', err);
        }
      });
    }
  }
  onUploadImage(file: File) {
    console.log(file);
    this.loading.set(true);
    this.photoService.uploadPhoto(file).subscribe({
      next: (photo: Photo) => {
        this.memberService.editMode.set(false);
        this.loading.set(false); 
        this.photos.set([...this.photos(), photo]);
      },
      error: (err: any) => {
        this.toastService.error('Failed to upload photo.');
        console.error('Error uploading photo:', err);
        this.loading.set(false);
      }
    });
  }
  setMainPhoto(photo: Photo) {
    this.photoService.setMainPhoto(photo.id).subscribe({
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
        const currentPhotos = this.photos();
        const reorderedPhotos = [
          photo,
          ...currentPhotos.filter(p => p.id !== photo.id)
        ];
        this.photos.set(reorderedPhotos);
      },
      error: (err: any) => {
        this.toastService.error('Failed to set main photo.');
        console.error('Error setting main photo:', err);
      }
    });
  }
  deletePhoto(photo: Photo) {
    this.photoService.deletePhoto(photo.id).subscribe({
      next: () => {
        this.photos.set(this.photos().filter(p => p.id !== photo.id));
      },
      error: (err: any) => {
        this.toastService.error('Failed to delete photo.');
        console.error('Error deleting photo:', err);
      }
    });
  }
}