# Sử dụng một hình ảnh cơ sở chứa Node.js
FROM node:20-alpine3.17

# Đặt thư mục làm việc
WORKDIR /frontend/src/app

# Sao chép tất cả tệp từ thư mục gốc dự án vào thư mục làm việc của hình ảnh
COPY . .


# Cài đặt các gói cần thiết bằng Yarn
RUN yarn install

# Biên dịch ứng dụng Angular

# Cổng mà ứng dụng Angular lắng nghe
EXPOSE 4200

# Lệnh để khởi động ứng dụng khi chạy container
CMD ["yarn", "start"]
