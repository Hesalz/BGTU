import { Column, CreateDateColumn, Entity, PrimaryGeneratedColumn, UpdateDateColumn } from 'typeorm';

@Entity({ name: 'configurations' })
export class Configuration {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Column({ name: 'owner_id', type: 'uniqueidentifier' })
  ownerId: string;

  @Column({ type: 'nvarchar', length: 100 })
  name: string;

  // Store the selected component identifiers in their chosen order.
  @Column({ name: 'component_ids', type: 'nvarchar', length: 'MAX' })
  componentIds: string;

  @CreateDateColumn({ name: 'created_at', type: 'datetime2' })
  createdAt: Date;

  @UpdateDateColumn({ name: 'updated_at', type: 'datetime2' })
  updatedAt: Date;
}
