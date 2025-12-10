<template>
  <div class="app-shell">
    <header class="app-header">
      <h1 class="app-title">KashCash Finder</h1>
      <p class="app-subtitle">Find KashCash-friendly businesses near you</p>
    </header>

    <main class="app-main">
      <HomePage />
    </main>

    <footer class="app-footer">
      <small>Early prototype · Mobile-first web view</small>
    </footer>
  </div>
</template>

<script setup>
import HomePage from './pages/HomePage.vue';
</script>



