import { Component, computed, input, model, output } from '@angular/core';

@Component({
  selector: 'app-paginator',
  imports: [],
  templateUrl: './paginator.html',
  styleUrl: './paginator.css'
})
export class Paginator {
  // input is readable only (value passed by parent and can't be changed by the child)
  // model is readable and writable
  pageNumber = model(1); // count here from the user
  pageSize = model(10); // count here from the user
  totalCount = input(0); // total items count from parent
  totalPages = input(0); // total pages from parent
  pageSizeOptions = input([5, 10, 20, 50]); // page size options from parent
  pageChange = output<{pageNumber: number, pageSize: number}>(); // event emitter to notify parent of page changes
  lastItemIndex = computed(() => { // used to show "Showing items X to Y of Z"
    return Math.min(this.pageNumber()* this.pageSize(), this.totalCount());
  })

  onPageChange(newPage?: number, pageSize?:EventTarget | null) {
    if(newPage) this.pageNumber.set(newPage);
    if(pageSize) {
      const size = Number((pageSize as HTMLSelectElement).value);
      this.pageSize.set(size);

    }

    this.pageChange.emit({
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize()
    })
  }
}
