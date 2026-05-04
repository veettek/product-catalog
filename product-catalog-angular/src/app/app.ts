import { Component, ViewChild } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { ProductForm } from './components/product-form/product-form';
import { ProductList } from './components/product-list/product-list';

@Component({
  selector: 'app-root',
  imports: [MatToolbarModule, MatCardModule, ProductForm, ProductList],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  @ViewChild(ProductList) listComponent!: ProductList;

  onProductAdded(): void {
    this.listComponent.load();
  }
}
