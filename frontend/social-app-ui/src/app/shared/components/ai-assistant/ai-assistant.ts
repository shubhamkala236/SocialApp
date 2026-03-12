import {
  Component,
  inject,
  signal,
  EventEmitter,
  Output,
  Input
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AiService, PostTone, AIPostResponse } from '../../../core/services/ai-service';
import { ThemeService } from '../../../core/services/theme-service';
import { ToastService } from '../../../core/services/toast.service';
import { Observable } from 'rxjs';

export interface AIResult {
  title: string;
  content: string;
}

type AIAction = 'generate' | 'improve' | 'rephrase' | 'summarize' | 'hook';

@Component({
  selector: 'app-ai-assistant',
  imports: [
    CommonModule,
    FormsModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
  ],
  templateUrl: './ai-assistant.html',
  styleUrl: './ai-assistant.scss',
})
export class AiAssistant {
  private aiService = inject(AiService);
  private toast = inject(ToastService);
  themeService = inject(ThemeService);
  // private cdr = inject(ChangeDetectorRef);
  showActions = signal(true);

  // Inputs from parent (current form values)
  @Input() currentTitle = '';
  @Input() currentContent = '';

  // Output — apply result to form
  @Output() applyResult = new EventEmitter<AIResult>();
  @Output() closePanel = new EventEmitter<void>();

  // State
  isOpen = signal(false);
  isLoading = signal(false);
  activeAction = signal<AIAction | null>(null);
  result = signal<AIPostResponse | null>(null);

  idea = '';
  selectedTone = signal<PostTone>('Casual');

  selectAction(action: AIAction) {
    this.activeAction.set(action);
    this.result.set(null);
    this.showActions.set(true);
  }


  readonly tones: PostTone[] = ['Casual', 'Professional', 'Funny', 'Inspirational', 'Storytelling'];

  readonly actions = [
    {
      id: 'generate' as AIAction,
      label: 'Generate',
      icon: 'auto_awesome',
      desc: 'Create a post from your idea',
      color: 'text-violet-400',
    },
    {
      id: 'improve' as AIAction,
      label: 'Improve',
      icon: 'trending_up',
      desc: 'Make your post better',
      color: 'text-blue-400',
    },
    {
      id: 'rephrase' as AIAction,
      label: 'Rephrase',
      icon: 'refresh',
      desc: 'Rewrite in a different way',
      color: 'text-green-400',
    },
    {
      id: 'summarize' as AIAction,
      label: 'Summarize',
      icon: 'compress',
      desc: 'Make it shorter & punchier',
      color: 'text-orange-400',
    },
    {
      id: 'hook' as AIAction,
      label: 'Add Hook',
      icon: 'bolt',
      desc: 'Make opening irresistible',
      color: 'text-pink-400',
    },
  ];

  selectTone(tone: PostTone) {
    this.selectedTone.set(tone);
  }

  // ai-assistant.component.ts
  run() {
    const action = this.activeAction();
    if (!action) return;

    this.isLoading.set(true);
    this.result.set(null);

    let obs$: Observable<AIPostResponse>;

    switch (action) {
      case 'generate':
        if (!this.idea.trim()) {
          this.toast.error('Please enter an idea first.');
          this.isLoading.set(false);
          return;
        }
        obs$ = this.aiService.generatePost({
          idea: this.idea,
          tone: this.selectedTone()
        });
        break;

      case 'improve':
        if (!this.currentContent.trim()) {
          this.toast.error('Write some content first.');
          this.isLoading.set(false);
          return;
        }
        obs$ = this.aiService.improvePost({
          title:   this.currentTitle,
          content: this.currentContent,
          tone:    this.selectedTone()
        });
        break;

      case 'rephrase':
        if (!this.currentContent.trim()) {
          this.toast.error('Write some content first.');
          this.isLoading.set(false);
          return;
        }
        obs$ = this.aiService.rephrasePost({
          content: this.currentContent,
          tone:    this.selectedTone()
        });
        break;

      case 'summarize':
        if (!this.currentContent.trim()) {
          this.toast.error('Write some content first.');
          this.isLoading.set(false);
          return;
        }
        obs$ = this.aiService.summarizePost(this.currentContent);
        break;

      case 'hook':
        if (!this.currentContent.trim()) {
          this.toast.error('Write some content first.');
          this.isLoading.set(false);
          return;
        }
        obs$ = this.aiService.makeHook(this.currentContent);
        break;

      default:
        this.isLoading.set(false);
        return;
    }

    obs$.subscribe({
      next: (res) => {
        console.log('✅ Setting result:', res);
        this.result.set(res);       // ✅ set signal
        this.isLoading.set(false);
        // this.cdr.detectChanges();   // ✅ force Angular to re-render
        console.log('✅ Result signal value:', this.result());
      },
      error: (err) => {
        console.error('❌ Error:', err);
        this.toast.error('AI request failed.');
        this.isLoading.set(false);
        // this.cdr.detectChanges();
      }
    });
  }

  applyToForm() {
    const r = this.result();
    if (!r) return;

    let title = r.title;
    let content = r.content;

    // ✅ If content looks like JSON (model returned nested JSON), parse it
    if (content.trim().startsWith('{') && content.trim().endsWith('}')) {
      try {
        const parsed = JSON.parse(content);
        if (parsed.title) title = parsed.title;
        if (parsed.content) content = parsed.content;
      } catch {
        // keep original content
      }
    }

    // this.applyResult.emit({ title, content });
    // this.result.set(null);
    // this.activeAction.set(null);
    // this.toast.success('AI suggestion applied! ✨');
    this.applyResult.emit({ title, content });
    this.result.set(null);
    this.activeAction.set(null);
    this.showActions.set(true); // ✅ show actions again after apply
    this.toast.success('AI suggestion applied! ✨');

  }

  tryAgain() {
    this.result.set(null);
    this.showActions.set(true); // ✅ go back to actions
  }

  close() {
    this.closePanel.emit();
  }
}
