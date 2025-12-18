import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';
import { LOCALE_ID } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeDa from '@angular/common/locales/da';
import { provideHttpClient } from '@angular/common/http';

registerLocaleData(localeDa);

bootstrapApplication(AppComponent, {
  ...appConfig,
  providers: [
    { provide: LOCALE_ID, useValue: 'da-DK' },
    provideRouter(routes),
    provideHttpClient(),
    ...(appConfig.providers || [])
  ]
}).catch(err => console.error(err));
