import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ProductForm } from './components/product-form/product-form';
import { ProductList } from './components/product-list/product-list';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ProductForm, ProductList],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('product-catalog-angular');
}
