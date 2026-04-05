import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Member } from '../../types/member';
import { PaginatedResult } from '../../types/pagination';

@Injectable({
  providedIn: 'root',
})
export class LikesService {
  private baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  likeIds = signal<string[]>([])

  toggleLike(targetMember: string){
    return this.http.post(`${this.baseUrl}likes/${targetMember}`,{});
  }

  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    let params = new HttpParams()
      .set('predicate', predicate)
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    return this.http.get<PaginatedResult<Member>>(`${this.baseUrl}likes`, { params });
  }

 getLikeIds()
 {
  return this.http.get<string[]>(`${this.baseUrl}likes/list`).subscribe(ids => this.likeIds.set(ids))
 };

  clearLikeIds(){
    this.likeIds.set([]);
  };

}
