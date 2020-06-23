import { map } from 'rxjs/operators';
import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  private setting = {
    element: {
      dynamicDownload: null as HTMLElement
    }
  }

  public httpClient: HttpClient;
  public products: ManufacturerGLNandProductCode[];
  public baseUrl: string;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.httpClient = http;
    this.baseUrl = baseUrl;

    this.products = [
      { manufacturerGLN: '8712423012424', productCode: '2121105' },
      { manufacturerGLN: '8712507900081', productCode: '22438310' },
      { manufacturerGLN: '8719333007694', productCode: '458004605' }
    ];
  }

  doGetData(manufacturerGLN: string, productCode: string) {
    this.httpClient.get<string>(this.baseUrl + 'uobdata/' + manufacturerGLN + '/' + encodeURIComponent(productCode)).subscribe(result => {
      console.log(result);
      this.dynamicDownloadJson(result);
    }, error => console.error(error));
  }

  doGetAllData() {
    this.httpClient.post<string>(this.baseUrl + 'uobdata', this.products).subscribe(result => {
      console.log(result);
      this.dynamicDownloadJson(result);
    }, error => console.error(error));
  }

  doGetTemplate(manufacturerGLN: string, productCode: string, application: string, applicationVersion: string) {
    const options: {
      headers?: HttpHeaders;
      observe: 'response';
      params?: HttpParams;
      reportProgress?: boolean;
      responseType: 'json';
      withCredentials?: boolean;
    } = {
      observe: 'response',
      responseType: 'blob' as 'json'
    };
    this.httpClient.get<Blob>(this.baseUrl + 'uobtemplate/' + manufacturerGLN + '/' + encodeURIComponent(productCode) + '/' + encodeURIComponent(application) + '/' + encodeURIComponent(applicationVersion), options).pipe(map((res: any) => {
      console.log(res);
      return {
        filename: 'Products.zip',
        data: res.body
      };
    }))
      .subscribe(res => {
        let url = window.URL.createObjectURL(res.data);
        let a = document.createElement('a');
        document.body.appendChild(a);
        a.setAttribute('style', 'display: none');
        a.href = url;
        a.download = res.filename;
        a.click();
        window.URL.revokeObjectURL(url);
        a.remove();
      });
  }

  doGetAllTemplates(application: string, applicationVersion: string) {
    const options: {
      headers?: HttpHeaders;
      observe: 'response';
      params?: HttpParams;
      reportProgress?: boolean;
      responseType: 'json';
      withCredentials?: boolean;
    } = {
      observe: 'response',
      responseType: 'blob' as 'json'
    };
    this.httpClient.post<Blob>(this.baseUrl + 'uobtemplate/' + encodeURIComponent(application) + '/' + encodeURIComponent(applicationVersion), this.products, options).pipe(map((res: any) => {
      return {
        filename: 'Products.zip',
        data: res.body
      };
    }))
      .subscribe(res => {
        let url = window.URL.createObjectURL(res.data);
        let a = document.createElement('a');
        document.body.appendChild(a);
        a.setAttribute('style', 'display: none');
        a.href = url;
        a.download = res.filename;
        a.click();
        window.URL.revokeObjectURL(url);
        a.remove();
      });
  }

  dynamicDownloadJson(result: string) {
    this.dyanmicDownloadByHtmlTag({
      fileName: 'ProductData.json',
      text: JSON.stringify(result)
    });
  }

  dynamicDownloadZip(result: string) {
    this.dyanmicDownloadByHtmlTag({
      fileName: 'Products.zip',
      text: result
    });
  }

  private dyanmicDownloadByHtmlTag(arg: {
    fileName: string,
    text: string
  }) {
    if (!this.setting.element.dynamicDownload) {
      this.setting.element.dynamicDownload = document.createElement('a');
    }
    const element = this.setting.element.dynamicDownload;
    const fileType = arg.fileName.indexOf('.json') > -1 ? 'text/json' : 'text/plain';
    element.setAttribute('href', `data:${fileType};charset=utf-8,${encodeURIComponent(arg.text)}`);
    element.setAttribute('download', arg.fileName);

    const event = new MouseEvent("click");
    element.dispatchEvent(event);
  }
}

interface ManufacturerGLNandProductCode {
  manufacturerGLN: string;
  productCode: string;
}
