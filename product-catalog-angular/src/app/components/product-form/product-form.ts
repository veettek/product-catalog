import { Component, EventEmitter, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-product-form',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './product-form.html',
  styleUrl: './product-form.scss',
})
export class ProductForm {
  @Output() productAdded = new EventEmitter<void>();

  private fb = inject(FormBuilder);
  private productService = inject(ProductService);
  private snackBar = inject(MatSnackBar);

  submitting = signal(false);

  form = this.fb.group({
    kod: ['', [Validators.required]],
    nazwa: ['', [Validators.required]],
    cena: [null as number | null, [Validators.required, Validators.min(0)]],
  });

  get kod() { return this.form.controls.kod; }
  get nazwa() { return this.form.controls.nazwa; }
  get cena() { return this.form.controls.cena; }

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    this.submitting.set(true);

    this.productService.add(this.form.value as any).subscribe({
      next: () => {
        this.snackBar.open('Produkt został dodany.', 'OK', {
          duration: 3000,
          panelClass: 'snack-success',
        });
        this.form.reset();
        this.submitting.set(false);
        this.productAdded.emit();
      },
      error: (err) => {
        const msg = err?.error?.error ?? 'Wystąpił błąd podczas dodawania.';
        this.snackBar.open(msg, 'OK', { duration: 4000 });
        this.submitting.set(false);
      }
    });
  }
}
