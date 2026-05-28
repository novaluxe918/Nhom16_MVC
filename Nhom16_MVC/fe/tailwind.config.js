/** @type {import('tailwindcss').Config} */
export default {
    content: [
        "./index.html",
        "./src/**/*.{js,ts,jsx,tsx}",
    ],
    theme: {
        extend: {
            colors: {
                "surface": "#f5f7f5",
                "on-surface": "#2c2f2e",
                "primary": "#006b0a",
                "on-surface-variant": "#595c5b",
                "surface-container-high": "#e0e3e1",
                "surface-container-lowest": "#ffffff",
                "primary-container": "#59ee50",
                "on-primary-container": "#005406",
                "on-primary": "#d2ffc4",
                "outline-variant": "#abaeac",
                "error": "#b02500"
            },
            fontFamily: {
                "display": ["Be Vietnam Pro", "sans-serif"],
                "body": ["Be Vietnam Pro", "sans-serif"],
            }
        },
    },
    plugins: [],
}