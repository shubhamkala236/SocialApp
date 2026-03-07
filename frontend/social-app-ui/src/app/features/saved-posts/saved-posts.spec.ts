import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SavedPosts } from './saved-posts';

describe('SavedPosts', () => {
  let component: SavedPosts;
  let fixture: ComponentFixture<SavedPosts>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SavedPosts]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SavedPosts);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
