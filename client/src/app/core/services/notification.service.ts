import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface Toast {
  id: string;
  message: string;
  type: 'success' | 'error' | 'warning' | 'info';
  autoDismiss?: boolean;
  timeout?: number;
}

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private readonly toasts = new BehaviorSubject<Toast[]>([]);
  public toasts$ = this.toasts.asObservable();

  showToast(
    message: string,
    type: 'success' | 'error' | 'warning' | 'info' = 'info',
    autoDismiss: boolean = true,
    timeout: number = 5000
  ): string {
    const id = this.generateUniqueId();
    const toast: Toast = {
      id,
      message,
      type,
      autoDismiss,
      timeout,
    };

    const currentToasts = this.toasts.getValue();
    this.toasts.next([...currentToasts, toast]);

    if (autoDismiss) {
      setTimeout(() => {
        this.removeToast(id);
      }, timeout);
    }

    return id;
  }

  removeToast(id: string): void {
    const currentToasts = this.toasts.getValue();
    this.toasts.next(currentToasts.filter((toast) => toast.id !== id));
  }

  clearAllToasts(): void {
    this.toasts.next([]);
  }

  private generateUniqueId(): string {
    return Date.now().toString(36) + Math.random().toString(36).substring(2);
  }
}
