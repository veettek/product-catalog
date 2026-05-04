import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatIconModule } from '@angular/material/icon';
import { Product } from '../../models/product.model';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-product-list',
  imports: [
    CommonModule,
    MatTableModule,
    MatProgressBarModule,
    MatIconModule,
  ],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss',
})
export class ProductList {
  products = signal<Product[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);

  displayedColumns = ['index', 'kod', 'nazwa', 'cena'];

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    console.log("load()");
    this.loading.set(true);
    this.error.set(null);
    this.productService.getAll().subscribe({
      next: (data) => {
        console.log("next");
        console.log(data);
        this.products.set(data);
        this.loading.set(false);
      },
      error: () => {
        console.log("error");
        this.error.set('Nie udało się pobrać produktów.');
        this.loading.set(false);
      }
    });
  }
}
