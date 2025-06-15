# Proje: University gRPC Library System

## Açıklama
Bu proje, bir üniversite kütüphanesi için kitap, öğrenci ve ödünç işlemlerini yöneten gRPC tabanlı bir istemci-sunucu uygulamasıdır. Tüm API tanımı Protocol Buffers ile yapılmıştır.

## Klasör Yapısı
```
/
├── university.proto
├── README.md
├── grpcurl-tests.md
├── images/
├── src/
│   ├── server/
│   └── client/
└── DELIVERY.md
```

## Kurulum ve Çalıştırma

1. Gerekli .NET SDK'yı kurun (örn: .NET 8) 
[https://dotnet.microsoft.com/en-us/download/dotnet/8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
2. Bağımlılıkları yükleyin ve projeleri derleyin:
   ```
   dotnet build src/server
   dotnet build src/client
   ```
3. Sunucuyu başlatın:
   ```
   dotnet run --project src/server
   ```
4. Ayrı bir terminalde istemciyi başlatın:
   ```
   dotnet run --project src/client
   ```

## Test
- Servisleri test etmek için [grpcurl](https://github.com/fullstorydev/grpcurl) kullanılabilir.
- Detaylı komutlar, örnekler ve güncel hata kontrolü senaryoları için `grpcurl-tests.md` dosyasına bakınız.
- Tüm servislerde NotFound, AlreadyExists, InvalidArgument gibi gRPC hata kodları ile gerçekçi hata yönetimi uygulanmıştır. Yanıtlar grpcurl çıktısı ile birebir uyumludur.

## Notlar
- Otomatik üretilen stub dosyaları (ör. obj/, bin/ altındaki .cs dosyaları) repoya eklenmemelidir.
- Sadece .proto dosyası kaynak olarak tutulur.
- Testler ve örnek çıktılar için grpcurl-tests.md dosyasına bakınız.
