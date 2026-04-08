# Project Completion Roadmap

Tai lieu nay tong hop cac hang muc nen lam tiep cho he thong WebShortlink, sap xep tu uu tien cao nhat den nhung cai co the lam dan. Muc tieu la lam theo nhieu dot nho, moi dot xong thi chay lai he thong, test lai, roi moi sang dot tiep theo.

## Cach dung tai lieu nay

- Lam theo thu tu uu tien tu tren xuong duoi.
- Moi hang muc nen duoc xu ly trong mot nhanh nho hoac mot dot rieng.
- Sau moi dot:
  - chay backend
  - chay frontend
  - test tay cac luong lien quan
  - ghi lai cai gi da on, cai gi con loi
- Khong nen sua qua nhieu nhom van de trong cung mot lan neu chua test xong.

## Muc tieu tong the

- He thong chay on dinh o local va de dua len moi truong that
- Giao dien user va admin dong bo, de dung, nhin ra san pham that
- Analytics co gia tri hon cho marketing
- Billing va plan feature ro rang hon
- Domain, bao mat va van hanh du an chac hon

---

## Phase 1 - Hoan thien nen tang bat buoc

Day la nhom viec nen uu tien cao nhat. Neu chua xong, khong nen mo rong tinh nang lon.

### 1. On dinh hoa cau hinh va cach chay du an

Muc tieu:
- backend va frontend chay on dinh
- config ro rang
- nguoi khac clone repo ve biet cach chay

Viec can lam:
- ra soat lai `appsettings.json`, `appsettings.Development.json`, `.env.example`
- thong nhat ten bien moi truong giua backend va frontend
- viet lai huong dan chay local ngan gon, de hieu
- kiem tra lai base URL backend/frontend khong bi lech
- dam bao startup production fail-fast dung nhu mong muon

Xong khi:
- clone repo moi van co the cau hinh va chay duoc
- khong con config nguy hiem de commit nham
- backend len duoc, frontend goi API dung

### 2. Khoa cac loi nghiep vu chinh

Muc tieu:
- cac luong can ban khong vo

Viec can lam:
- test dang ky, dang nhap, refresh token, logout
- test tao link, sua link, tam dung, xoa link
- test redirect public
- test analytics overview va link detail
- test nang cap goi, tao payment pending, admin approve
- test custom domain verify

Xong khi:
- moi luong chinh deu co checklist test tay
- loi gap o cac luong tren duoc fix xong

### 3. Don sach code va thong diep loi

Muc tieu:
- code de doc hon
- thong diep cho user/admin de hieu hon

Viec can lam:
- ra soat cac chuoi tieng Viet bi loi encoding
- thong nhat wording giua frontend va backend
- doi cac ten bien, ten section giao dien dang kho hieu
- loai bo log, comment, text tam khong can thiet

Xong khi:
- UI khong con text loi font
- response loi chinh nhin ro rang, nhat quan

---

## Phase 2 - Hoan thien trai nghiem san pham

### 4. Dong bo toan bo giao dien user

Muc tieu:
- user area nhin dong bo, khong moi trang mot kieu

Viec can lam:
- thong nhat page header, section card, toolbar, button, badge
- tiep tuc lam dep:
  - dashboard
  - links
  - analytics
  - billing
  - domains
  - profile
  - create link
  - link detail
- bo sung empty state, loading state, error state dep hon

Xong khi:
- vao user area thay cung mot ngon ngu thiet ke
- cac trang khong con trong, tho, hoac lech tong

### 5. Dong bo toan bo giao dien admin

Muc tieu:
- admin area nhin ra he thong quan tri that

Viec can lam:
- thong nhat admin layout, topbar, sidebar, page header
- lam dep dashboard admin
- lam lai table/list cho:
  - users
  - links
  - payments
  - reports
  - security
  - audit logs
- uu tien thong tin quan trong bang hierarchy ro rang

Xong khi:
- admin area dong bo, de quet du lieu, de thao tac

### 6. Nang cap UX cho billing

Muc tieu:
- nguoi dung de hieu goi nao hop voi minh
- de nang cap hon

Viec can lam:
- pricing page dep hon
- checkout page ro hon
- payment history ro trang thai hon
- hien ro current plan, pending payment, benefit cua tung goi

Xong khi:
- flow nang cap goi de hieu, de thao tac

---

## Phase 3 - Nang cap tinh nang co gia tri cao

### 7. Nang cap analytics nguon truy cap

Muc tieu:
- analytics co gia tri that, khong chi dem click

Viec can lam:
- parse `referrer` thanh nhom nguon de doc:
  - Facebook
  - TikTok
  - Instagram
  - Google
  - YouTube
  - Zalo
  - Direct
- ho tro `utm_source`, `utm_medium`, `utm_campaign`
- hien thi top source theo nhom thay vi chuoi raw
- can nhac luu campaign metadata vao click event

Xong khi:
- user biet click den tu dau mot cach de hieu

### 8. Nang cap rule engine thanh smart routing

Muc tieu:
- bien shortlink thanh cong cu marketing that

Viec can lam:
- bo sung chia traffic theo phan tram
- cai thien rotation rule
- can nhac A/B testing cho nhieu target URL
- ghi nhan ket qua tung rule vao analytics

Xong khi:
- co the chay chien dich don gian bang link thong minh

### 9. Nang cap custom domain

Muc tieu:
- custom domain chac hon, gan thuc te hon

Viec can lam:
- bo sung DNS TXT verification
- hien thi huong dan verify ro hon
- bo sung health check cho domain
- canh bao domain mat verify hoac sai cau hinh

Xong khi:
- domain verification va trang thai domain dang tin hon

---

## Phase 4 - Tinh nang co the giup ban hang hoac demo manh hon

### 10. QR code cho tung link

Gia tri:
- de demo
- de in an
- hop voi user marketing

Viec can lam:
- sinh QR code cho shortlink
- download PNG/SVG
- them vao link detail

### 11. Link in bio / microsite mini

Gia tri:
- de mo rong san pham
- hop voi creator va shop online

Viec can lam:
- tao mot page bio co nhieu nut
- cho phep gan analytics
- cho phep gan custom domain neu goi ho tro

### 12. Webhook / API key

Gia tri:
- hop voi goi Pro/Plus
- tang kha nang tich hop

Viec can lam:
- tao API key cho user
- webhook khi:
  - tao link
  - co payment moi
  - domain verified
  - click tang bat thuong

### 13. Export va bao cao dinh ky

Gia tri:
- hop voi user doanh nghiep

Viec can lam:
- export CSV/PDF theo bo loc
- gui email bao cao dinh ky
- lich bao cao hang tuan/hang thang

---

## Phase 5 - Bao mat va van hanh nang cao

### 14. Bao mat click va fraud detection

Muc tieu:
- giam click rac, bot va hanh vi bat thuong

Viec can lam:
- nang cap bot detection
- blacklist IP/co quoc gia/co ASN
- rate-limit theo slug va host
- canh bao khi click tang dot bien

### 15. Monitoring va observability

Muc tieu:
- de van hanh hon khi he thong lon len

Viec can lam:
- bo sung structured logging
- bo sung metrics ro hon cho queue, redirect, payment
- tach health check co y nghia hon
- can nhac dashboard van hanh

### 16. Test va CI

Muc tieu:
- sua toi dau chac toi do

Viec can lam:
- them integration test cho auth, links, billing, domain
- them frontend smoke test cho cac luong chinh
- them build check trong CI

Xong khi:
- moi lan sua lon deu de verify hon

---

## Thu tu nen lam de "lam den dau chay den do"

Neu ban muon di thuc dung, nen theo thu tu sau:

1. On dinh config + cach chay du an
2. Test va fix luong chinh
3. Don sach UI/text/error message
4. Dong bo user area
5. Dong bo admin area
6. Nang cap billing UX
7. Nang cap analytics source + UTM
8. Nang cap smart routing / A-B testing
9. Nang cap custom domain
10. Them QR code
11. Them webhook / API key
12. Them observability + test + CI

---

## Checklist sau moi dot

Sau moi nhom viec, hay tu check:

- backend co chay len khong
- frontend co chay len khong
- co vo route nao dang dung khong
- login user/admin con on khong
- API co bi loi contract frontend/backend khong
- UI co bi vo mobile/tablet khong
- co text loi font hoac label thieu ten khong
- co can cap nhat lai README/docs khong

---

## De xuat 3 huong nen uu tien neu muon thay hieu qua nhanh

Neu chi chon 3 huong de lam tiep ngay, minh de xuat:

1. Dong bo toan bo user area va admin area
2. Nang cap analytics source + UTM
3. Nang cap smart routing va pricing/billing flow

Ly do:
- nhin ra san pham hon
- demo tot hon
- de mo rong thanh san pham thuc te hon

---

## Ghi chu

- Tai lieu nay uu tien huong di thuc te, khong co gang liet ke moi y tuong co the co.
- Khi bat dau mot phase moi, nen tao task con nho hon de tranh sua qua nhieu thu mot luc.
- Neu can, co the tach tiep thanh:
  - roadmap backend
  - roadmap frontend
  - roadmap business feature
