# BNS360 🚀

Welcome to **BNS360**, a powerful and comprehensive system designed to streamline the management of businesses, crafts, jobs, profiles, categories, feedback, and so much more. If you’re ready to manage everything in one place, from user authentication to favorite businesses, you’re in the right place! 🎉

## Features 🔥

**BNS360** is built with efficiency and usability at its core. Here's what you can do with it:

### 1. Authentication & Authorization 🔐
- **Register, Login, and User Management**: Seamlessly handle user authentication with endpoints for user registration, login, and role management.
- **JWT Integration**: Secure token-based authentication to keep your application safe and sound.
- **Password Management**: Forgot your password? No worries, you can reset it, change it, and verify your account through email.

### 2. Business Management 🏢
- **Add/Edit Businesses**: Manage businesses with ease. Create new businesses or update the existing ones.
- **Business Categories**: Set businesses into categories for better organization and user experience.
- **Business-Category Linking**: Link businesses to specific categories to simplify searches and improve navigation.
- **Promotions for Customers**: Keep your customers in the loop with awesome promotions!

### 3. Categories 📂
- **Category Management**: Add, edit, and get all categories to maintain a well-structured and organized database.

### 4. Crafts & Craftmen 🛠️
- **Craft Management**: Add crafts and link them to specific categories or craftsmen.
- **Craftsmen Profiles**: Create profiles for skilled workers and match them to the crafts they specialize in.
- **Availability & Updates**: Update their status, availability, and manage their relationships with different jobs.

### 5. Jobs 🏗️
- **Job Listings**: Post new job listings and search for the perfect job that fits your skills.
- **Saved Jobs**: Save job listings for future reference so you don’t miss any opportunity.
- **Job Applications**: Apply for jobs directly from the platform with a single click.

### 6. Favorites ❤️
- **Save Favorites**: Add businesses, craftsmen, or properties to your favorites for quick access later.
- **Unfavorite Items**: Don’t like something anymore? Simply remove it from your list.

### 7. Feedback 📢
- **Provide Feedback**: Share your feedback on businesses, jobs, and services.
- **Feedback Management**: Review and respond to feedback to improve user experience.

### 8. Property Listings 🏡
- **Add & Manage Properties**: List your properties, whether for rent or sale, and manage the listings directly.
- **Property Details**: Get detailed information about each property.

### 9. User Profiles 👤
- **Update Profiles**: Personalize user profiles with updates, changes, and relevant details.
- **Manage Contact Information**: Keep contact details up-to-date for easy communication.

---

## Tech Stack 🛠️

- **Backend**: ASP.NET Core (C#) with Clean Architecture
- **Database**: SQL Server for robust and scalable data management
- **Authentication**: JWT (JSON Web Token) for secure, token-based authentication
- **Cloud Services**: Integrated with **Cloudinary** for image uploads, **SMTP** for email verification

---

## How to Run 💻

1. **Clone the repo**:
   ```bash
   git clone https://github.com/your-repo/BNS360.git
   ```
2. **Navigate to the project directory**:
   ```bash
   cd BNS360
   ```
3. **Install dependencies**:
   ```bash
   dotnet restore
   ```
4. **Run the project**:
   ```bash
   dotnet run
   ```

---

## API Endpoints 📡

Our API provides the following endpoints to interact with the system:

- **Auth**: `/api/auth/`
- **Business**: `/api/business/`
- **Category**: `/api/category/`
- **Craft**: `/api/craft/`
- **Craftsmen**: `/api/craftsmen/`
- **Favorite**: `/api/favorite/`
- **Feedback**: `/api/feedback/`
- **Job**: `/api/job/`
- **Profile**: `/api/profile/`
- **Property**: `/api/property/`
- **Saved Jobs**: `/api/savedjobs/`
- **UserRole**: `/api/userrole/`

Each endpoint is designed to be RESTful, providing you with the most straightforward way to interact with the application. For detailed API documentation, please refer to the Swagger UI integrated within the project.

---

## Why BNS360? 🎯

With **BNS360**, we aimed to simplify business and job management, offering a full 360-degree view of operations, from categories and craftsmen to properties and feedback. The goal is to provide a unified solution for everyone, whether you're a business owner, a job seeker, or just browsing through categories and listings. We’ve put in all our love and passion into building something that’s powerful yet easy to use. 💪

