import { Component, input, Output, output, signal } from '@angular/core';

@Component({
  selector: 'app-image-upload',
  imports: [],
  templateUrl: './image-upload.html',
  styleUrl: './image-upload.css'
})
export class ImageUpload {
  protected imageSrc = signal<string | ArrayBuffer | null | undefined>(null); // holds the preview image data (to show in the UI)
  protected isDragging = false; // can be used to style the drop area (UI/UX)
  private fileToUpload: File | null = null; // Stores the selected file to send later (to API or parent component).
  uploadFile = output<File>(); // Emits the file to the parent component when user clicks “Upload image”.
  loading = input<boolean>(false);
  disabled: any;

  onDragOver(event: DragEvent) {
    // stops the browser’s default behavior (which would try to open the image file in the browser tab).
    event.preventDefault();
    // means Don’t let this event travel upward to parent elements in the DOM tree (because they might have their own drag-and-drop handlers).
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

    // dataTransfer property of the DragEvent contains the files that were dropped.
    // take the first file and set to the preview function to show the image in the UI.
    // set it to the fileToUpload variable to emit it later when user clicks the Upload button to the backend.
    if(event.dataTransfer && event.dataTransfer.files.length > 0) {
      const file = event.dataTransfer.files[0];
      this.previewImage(file);
      this.fileToUpload = file;
    }
  }
  // this mainly to convert the image to image data (string represent the image) and set it imgSrc signal to show in the UI as preview.
  private previewImage(file: File) {
    // FileReader is a built-in JavaScript object that can read files (like images, PDFs, etc.) from your computer and turn them into data that can be displayed in the browser.
    // It’s like a helper that can read files (like images, PDFs, text files) from your computer and turn them into a format your webpage can use — for example, a string that represents the image.
    const reader = new FileReader();
    // When FileReader finishes reading the file, it triggers an onload event.
    reader.onload = (e) => {
      // e.target?.result contains the file data as a Data URL (a text representation of the image).
      // so in template you can assign it to the src of an <img> tag to show the image preview.
      this.imageSrc.set(e.target?.result);
    };
    // This tells the FileReader to actually start reading the file and convert it into a Data URL (a text representation of the image).
    // This is asynchronous — it takes a moment, and when it’s done, the onload above is triggered.
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