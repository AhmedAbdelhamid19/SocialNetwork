import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Photo } from '../../types/member';

@Injectable({
  providedIn: 'root'
})
export class PhotoService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  getMemberPhotos(id: string) {
    return this.http.get<Photo[]>(this.baseUrl + `photos/${id}`);
  }

  uploadPhoto(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<Photo>(this.baseUrl + 'photos/add-photo', formData);
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + `photos/set-main-photo/${photoId}`, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + `photos/delete-photo/${photoId}`);
  }
}
