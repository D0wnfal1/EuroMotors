import { Injectable } from '@angular/core';
import { HttpResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class HttpCacheService {
  private readonly cache = new Map<string, CacheEntry>();
  private readonly DEFAULT_MAX_AGE = 5 * 60 * 1000;
  constructor() {}

  get(url: string): HttpResponse<any> | undefined {
    const entry = this.cache.get(url);
    if (!entry) {
      return undefined;
    }

    const isExpired = Date.now() - entry.entryTime > entry.maxAge;
    if (isExpired) {
      this.delete(url);
      return undefined;
    }

    return entry.response;
  }

  set(
    url: string,
    response: HttpResponse<any>,
    maxAge: number = this.DEFAULT_MAX_AGE
  ): void {
    const entry: CacheEntry = {
      url,
      response,
      entryTime: Date.now(),
      maxAge,
    };
    this.cache.set(url, entry);
  }

  delete(url: string): void {
    this.cache.delete(url);
  }

  clear(): void {
    this.cache.clear();
  }
}

interface CacheEntry {
  url: string;
  response: HttpResponse<any>;
  entryTime: number;
  maxAge: number;
}
