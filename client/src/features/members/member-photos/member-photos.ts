import { Component, inject, OnInit, Signal, signal } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { Photo } from '../../../types/member';
import { ImageUpload } from "../../../shared/image-upload/image-upload";

@Component({
  selector: 'app-member-photos',
  imports: [ImageUpload],
  templateUrl: './member-photos.html',
  styleUrl: './member-photos.css'
})
export class MemberPhotos implements OnInit {
  protected memberService = inject(MemberService);
  private route = inject(ActivatedRoute);
  protected photos = signal<Photo[]>([]);
  protected loading = signal(false);

  constructor() {
  }
  ngOnInit(): void {
    // instead of setting up your subscription in the constructor, you should use ngOnInit:
    // ngOnInit is the standard Angular lifecycle hook for performing side effects like HTTP calls.
    // The constructor should only handle dependency injection, not logic.
    //When going out to get data from an API, it's typically done in the ng on init.
    const memberId = this.route.parent?.snapshot.paramMap.get('id')!;
    if(memberId) {
      this.memberService.getMemberPhotos(memberId).subscribe({
        next: photos => this.photos.set(photos)
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
}