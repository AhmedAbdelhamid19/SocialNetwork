import { Component, input, Output, output, signal } from '@angular/core';

@Component({
  selector: 'app-image-upload',
  imports: [],
  templateUrl: './image-upload.html',
  styleUrl: './image-upload.css'
})
export class ImageUpload {
  protected imageSrc = signal<string | ArrayBuffer | null | undefined>(null);
  protected isDragging = false;
  private fileToUpload: File | null = null;
  uploadFile = output<File>();
  loading = input<boolean>(false);
  disabled: any;

  onFileSelected(event: Event) {
  const input = event.target as HTMLInputElement;
  if (!input.files?.length) return;

  const file = input.files[0];
  this.previewImage(file);
  this.fileToUpload = file;
  }
  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }
  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }
  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
    if(event.dataTransfer && event.dataTransfer.files.length > 0) {
      const file = event.dataTransfer.files[0];
      this.previewImage(file);
      this.fileToUpload = file;
    }
  }
  private previewImage(file: File) {
    const reader = new FileReader();
    reader.onload = (e) => {
      // when you read file, convert it to data url and set to imageSrc signal
      // in template, we will bind imageSrc to img tag to show preview
      this.imageSrc.set(e.target?.result);
    };
    reader.readAsDataURL(file);
  }
  onCancel() {
    this.imageSrc.set(null);
    this.fileToUpload = null;
  }
  onUpload() {
    if(this.fileToUpload) {
      this.uploadFile.emit(this.fileToUpload);
    }
  }
}