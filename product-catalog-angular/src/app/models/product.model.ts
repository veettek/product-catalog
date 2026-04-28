export interface Product {
  id: string;
  kod: string;
  nazwa: string;
  cena: number;
}

export interface ProductCreateDto {
  kod: string;
  nazwa: string;
  cena: number;
}
