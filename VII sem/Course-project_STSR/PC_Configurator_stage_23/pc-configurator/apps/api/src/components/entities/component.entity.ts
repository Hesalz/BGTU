import {
  Column, CreateDateColumn, Entity, JoinColumn, ManyToOne,
  PrimaryGeneratedColumn, UpdateDateColumn,
} from 'typeorm';
import { Category } from './category.entity';

@Entity({ name: 'components' })
export class Component {
  @PrimaryGeneratedColumn('uuid')
  id: string;

  // Legacy-readable category value is retained for compatibility with the initial schema.
  @Column({ type: 'nvarchar', length: 40 })
  category: string;

  @Column({ name: 'category_id', type: 'uniqueidentifier', nullable: true })
  categoryId: string | null;

  @ManyToOne(() => Category, (category) => category.components, { nullable: true, onDelete: 'SET NULL' })
  @JoinColumn({ name: 'category_id' })
  categoryEntity: Category | null;

  @Column({ type: 'nvarchar', length: 120 })
  manufacturer: string;

  @Column({ type: 'nvarchar', length: 180 })
  model: string;

  @Column({ type: 'nvarchar', length: 'MAX', nullable: true })
  description: string | null;

  @Column({ type: 'decimal', precision: 12, scale: 2, nullable: true })
  price: number | null;

  @Column({ type: 'int', nullable: true, name: 'power_w' })
  powerW: number | null;

  @Column({ type: 'nvarchar', length: 'MAX', nullable: true })
  specifications: string | null;

  @Column({ type: 'nvarchar', length: 1000, nullable: true, name: 'image_url' })
  imageUrl: string | null;

  @Column({ type: 'nvarchar', length: 'MAX', nullable: true, name: 'image_urls' })
  imageUrls: string | null;

  @Column({ type: 'nvarchar', length: 'MAX', nullable: true, name: 'price_offers' })
  priceOffers: string | null;

  @Column({ type: 'nvarchar', length: 80, nullable: true, name: 'source_name' })
  sourceName: string | null;

  @Column({ type: 'nvarchar', length: 180, nullable: true, name: 'external_id' })
  externalId: string | null;

  @Column({ type: 'nvarchar', length: 180, nullable: true, name: 'manufacturer_part_number' })
  manufacturerPartNumber: string | null;

  @Column({ type: 'nvarchar', length: 120, nullable: true, name: 'price_source' })
  priceSource: string | null;

  @Column({ type: 'nvarchar', length: 1000, nullable: true, name: 'price_url' })
  priceUrl: string | null;

  @Column({ type: 'datetime2', nullable: true, name: 'price_updated_at' })
  priceUpdatedAt: Date | null;

  @Column({ type: 'nvarchar', length: 120, nullable: true, name: 'image_source' })
  imageSource: string | null;

  @CreateDateColumn({ name: 'created_at', type: 'datetime2' })
  createdAt: Date;

  @UpdateDateColumn({ name: 'updated_at', type: 'datetime2' })
  updatedAt: Date;
}
