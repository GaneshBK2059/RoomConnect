# React + TypeScript + Vite

This template provides a minimal setup to get React working in Vite with HMR and some ESLint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Babel](https://babeljs.io/) (or [oxc](https://oxc.rs) when used in [rolldown-vite](https://vite.dev/guide/rolldown)) for Fast Refresh
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/) for Fast Refresh

## React Compiler

The React Compiler is not enabled on this template because of its impact on dev & build performances. To add it, see [this documentation](https://react.dev/learn/react-compiler/installation).

## Expanding the ESLint configuration

If you are developing a production application, we recommend updating the configuration to enable type-aware lint rules:

```js
export default defineConfig([
  globalIgnores(['dist']),
  {
    files: ['**/*.{ts,tsx}'],
    extends: [
      // Other configs...

      // Remove tseslint.configs.recommended and replace with this
      tseslint.configs.recommendedTypeChecked,
      // Alternatively, use this for stricter rules
      tseslint.configs.strictTypeChecked,
      // Optionally, add this for stylistic rules
      tseslint.configs.stylisticTypeChecked,

      // Other configs...
    ],
    languageOptions: {
      parserOptions: {
        project: ['./tsconfig.node.json', './tsconfig.app.json'],
        tsconfigRootDir: import.meta.dirname,
      },
      // other options...
    },
  },
])
```

You can also install [eslint-plugin-react-x](https://github.com/Rel1cx/eslint-react/tree/main/packages/plugins/eslint-plugin-react-x) and [eslint-plugin-react-dom](https://github.com/Rel1cx/eslint-react/tree/main/packages/plugins/eslint-plugin-react-dom) for React-specific lint rules:

```js
// eslint.config.js
import reactX from 'eslint-plugin-react-x'
import reactDom from 'eslint-plugin-react-dom'

export default defineConfig([
  globalIgnores(['dist']),
  {
    files: ['**/*.{ts,tsx}'],
    extends: [
      // Other configs...
      // Enable lint rules for React
      reactX.configs['recommended-typescript'],
      // Enable lint rules for React DOM
      reactDom.configs.recommended,
    ],
    languageOptions: {
      parserOptions: {
        project: ['./tsconfig.node.json', './tsconfig.app.json'],
        tsconfigRootDir: import.meta.dirname,
      },
      // other options...
    },
  },
])
```

---

# Frontend Architecture Guide

We use a **Feature-Driven Architecture** to ensure scalability as the React application grows.

Here is a guide on how the structure works and where to put files:

```text
src/
├── assets/         # Static global assets (images, SVGs)
├── components/     # Reusable, domain-agnostic components
│   ├── ui/         # Generic UI elements (Button, Input, Card)
│   ├── layout/     # Structural components (Navbar, Sidebar, Footer)
│   └── forms/      # Reusable form elements
├── features/       # Feature-driven modules
│   └── auth/       # Everything related to Authentication
│       ├── api/    # API calls for auth
│       ├── components/ # Auth-specific components
│       ├── context/ # Auth global state
│       └── types/  # Auth TypeScript interfaces
├── pages/          # Page-level components that tie routes together
├── services/       # Global services (like the core api.ts Axios instance)
├── App.tsx         # Main entry and routing
├── index.css       # Global styles
└── main.tsx        # React DOM entry
```

## Key Rules for the Structure

### 1. The `features/` Directory
When building a new major feature (e.g., "Rooms", "Bookings", "Users"), create a new folder inside `src/features/`. All code specific to that feature goes inside its folder, grouped by:
*   `api/`: API calls to the backend
*   `components/`: React components only used by this feature
*   `types/`: TypeScript definitions for this feature
*   `hooks/`: Custom React hooks for this feature
*   `context/`: Context for this feature (if needed)

This keeps everything related to one domain together, avoiding jumping across the entire `src/` folder for a single feature.

### 2. The `components/` Directory
Only place components here if they are **truly reusable across different features**.
*   `components/ui/`: base elements (Buttons, Inputs).
*   `components/layout/`: shells and structural elements (Navbars, Footers).

### 3. The `pages/` Directory
Files in `pages/` should contain very little logic. They should primarily just import components from `features/` or `components/layout/` and stitch them together to form a complete view for a specific route.
