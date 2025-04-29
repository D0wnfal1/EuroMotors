import {
  ApplicationConfig,
  provideZoneChangeDetection,
  provideAppInitializer,
  inject,
  isDevMode,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import {
  provideHttpClient,
  withInterceptors,
  withFetch,
  HTTP_INTERCEPTORS,
} from '@angular/common/http';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { loadingInterceptor } from './core/interceptors/loading.interceptor';
import { InitService } from './core/services/init.service';
import { lastValueFrom } from 'rxjs';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import {
  provideHttpCache,
  httpCacheInterceptor,
} from './core/cache/http-cache.provider';
import { CacheInterceptor } from './core/interceptors/cache.interceptor';
import { provideServiceWorker } from '@angular/service-worker';

function initializeApp() {
  const initService = inject(InitService);
  return lastValueFrom(initService.init()).finally(() => {
    const splash = document.getElementById('initial-splash');
    if (splash) {
      splash.remove();
    }
  });
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideAnimationsAsync(),
    provideHttpCache(),
    provideHttpClient(
      withFetch(),
      withInterceptors([
        errorInterceptor,
        loadingInterceptor,
        authInterceptor,
        httpCacheInterceptor,
      ])
    ),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: CacheInterceptor,
      multi: true,
    },
    provideAppInitializer(initializeApp),
    provideServiceWorker('ngsw-worker.js', {
      enabled: !isDevMode(),
      registrationStrategy: 'registerWhenStable:30000',
    }),
  ],
};
