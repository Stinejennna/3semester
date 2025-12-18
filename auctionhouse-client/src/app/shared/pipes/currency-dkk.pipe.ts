import { Pipe, PipeTransform } from '@angular/core';
import { CurrencyPipe } from '@angular/common';

@Pipe({
  name: 'currencyDkk',
  standalone: true,
  pure: true
})
export class CurrencyDkkPipe implements PipeTransform {
  private currencyPipe = new CurrencyPipe('da-DK');

  transform(value: number | null | undefined): string | null {
    if (value == null) return null;
    return this.currencyPipe.transform(value, 'DKK', 'symbol', '1.0-0', 'da-DK');
  }
}
