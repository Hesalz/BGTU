import { Request } from 'express';
export type AuthUser = { sub: string; email: string; role: 'USER' | 'ADMIN' };
export type AuthRequest = Request & { user?: AuthUser };
