import api from './axios';
import type { ChangePasswordPayload } from './types';

export const AccountService = {
  async changePassword(payload: ChangePasswordPayload): Promise<void> {
    await api.post('/api/Account/change-password', payload);
  }
};