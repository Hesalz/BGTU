import { Column, CreateDateColumn, Entity, PrimaryGeneratedColumn, UpdateDateColumn } from 'typeorm';

export type UserRole = 'USER' | 'ADMIN';

@Entity({ name: 'users' })
export class User {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({ type: 'nvarchar', length: 254, unique: true })
  email: string;

  @Column({ name: 'password_hash', type: 'nvarchar', length: 255, select: false })
  passwordHash: string;

  @Column({ type: 'nvarchar', length: 80, nullable: true })
  displayName: string | null;

  @Column({ type: 'nvarchar', length: 10, default: 'USER' })
  role: UserRole;

  @CreateDateColumn({ name: 'created_at', type: 'datetime2' })
  createdAt: Date;

  @UpdateDateColumn({ name: 'updated_at', type: 'datetime2' })
  updatedAt: Date;
}
