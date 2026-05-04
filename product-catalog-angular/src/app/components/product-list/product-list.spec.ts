import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';

import { ProductList } from './product-list';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product.model';

describe('ProductList', () => {
  let fixture: ComponentFixture<ProductList>;
  let component: ProductList;
  let serviceSpy: jasmine.SpyObj<ProductService>;

  const mockProducts: Product[] = [
    { id: '1', kod: 'EL-001', nazwa: 'Laptop Lenovo', cena: 2949.50 },
    { id: '2', kod: 'EL-002', nazwa: 'Redmi GT2', cena: 1999 },
  ];

  beforeEach(async () => {
    serviceSpy = jasmine.createSpyObj('ProductService', ['getAll']);
    serviceSpy.getAll.and.returnValue(of(mockProducts));

    await TestBed.configureTestingModule({
      imports: [ProductList, ],
      providers: [{ provide: ProductService, useValue: serviceSpy }],
    }).compileComponents();

    fixture = TestBed.createComponent(ProductList);
    component = fixture.componentInstance;
  });

  // -------------------------------------------------------------------------
  // Initialization
  // -------------------------------------------------------------------------

  it('should create', () => {
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should call load() on init', () => {
    fixture.detectChanges();
    expect(serviceSpy.getAll).toHaveBeenCalledTimes(1);
  });

  // -------------------------------------------------------------------------
  // Loading state
  // -------------------------------------------------------------------------

  it('should set loading=false after data arrives', () => {
    fixture.detectChanges();
    expect(component.loading()).toBeFalse();
  });

  // -------------------------------------------------------------------------
  // Correct data
  // -------------------------------------------------------------------------

  it('should populate products array after successful load', () => {
    fixture.detectChanges();
    expect(component.products()).toEqual(mockProducts);
  });

  it('should render a mat-table row for each product', () => {
    fixture.detectChanges();
    const rows = fixture.debugElement.queryAll(By.css('mat-row, tr[mat-row]'));
    expect(rows.length).toBe(mockProducts.length);
  });

  it('should display product code in the table', () => {
    fixture.detectChanges();
    const compiled: HTMLElement = fixture.nativeElement;
    expect(compiled.textContent).toContain('EL-001');
    expect(compiled.textContent).toContain('EL-002');
  });

  it('should display product name in the table', () => {
    fixture.detectChanges();
    const compiled: HTMLElement = fixture.nativeElement;
    expect(compiled.textContent).toContain('Laptop Lenovo');
    expect(compiled.textContent).toContain('Redmi GT2');
  });

  // -------------------------------------------------------------------------
  // Empty list
  // -------------------------------------------------------------------------

  it('should show empty message when product list is empty', () => {
    serviceSpy.getAll.and.returnValue(of([]));
    fixture.detectChanges();

    const compiled: HTMLElement = fixture.nativeElement;
    expect(compiled.textContent).toContain('Brak produktów w katalogu');
  });

  it('should not render table when product list is empty', () => {
    serviceSpy.getAll.and.returnValue(of([]));
    fixture.detectChanges();

    const table = fixture.debugElement.query(By.css('table'));
    expect(table).toBeNull();
  });

  // -------------------------------------------------------------------------
  // API error
  // -------------------------------------------------------------------------

  it('should set error message when API call fails', () => {
    serviceSpy.getAll.and.returnValue(throwError(() => new Error('Network error')));
    fixture.detectChanges();

    expect(component.error()).toBeTruthy();
  });

  it('should display error message in template when API call fails', () => {
    serviceSpy.getAll.and.returnValue(throwError(() => new Error('Network error')));
    fixture.detectChanges();

    const compiled: HTMLElement = fixture.nativeElement;
    expect(compiled.textContent).toContain('Nie udało się pobrać produktów');
  });

  it('should set loading=false after error', () => {
    serviceSpy.getAll.and.returnValue(throwError(() => new Error('Network error')));
    fixture.detectChanges();

    expect(component.loading()).toBeFalse();
  });

  // -------------------------------------------------------------------------
  // Reloading
  // -------------------------------------------------------------------------

  it('should reload products when load() is called again', () => {
    fixture.detectChanges();
    component.load();

    expect(serviceSpy.getAll).toHaveBeenCalledTimes(2);
  });

  it('should clear error before reloading', () => {
    serviceSpy.getAll.and.returnValue(throwError(() => new Error()));
    fixture.detectChanges();
    expect(component.error()).toBeTruthy();

    serviceSpy.getAll.and.returnValue(of(mockProducts));
    component.load();
    fixture.detectChanges();

    expect(component.error()).toBeNull();
  });
});
