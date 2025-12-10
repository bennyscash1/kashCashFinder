<template>
  <section class="home">
    <header class="page-header">
      <h2 class="page-title">Company Directory</h2>
      <p class="page-subtitle">Search and browse a mini directory of companies.</p>
    </header>

    <div class="search-wrapper">
      <input
        v-model="search"
        class="search-input"
        type="text"
        placeholder="Search by company name (e.g. Osher Ad, McDonald's)..."
      />
    </div>

    <main>
      <div class="cards-grid">
        <article
          v-for="company in filteredCompanies"
          :key="company.id"
          class="card"
        >
          <div class="card-logo-wrap">
            <img
              :src="company.logo"
              :alt="`${company.name} logo`"
              class="card-logo"
            />
          </div>
          <h3 class="card-name">{{ company.name }}</h3>
          <p class="card-tagline">{{ company.tagline }}</p>
        </article>
      </div>

      <p v-if="!filteredCompanies.length" class="no-results">
        No companies match your search.
      </p>
    </main>
  </section>
</template>

<script setup>
import { computed, onMounted, ref, watch } from 'vue';
import debounce from 'lodash.debounce';

const search = ref('');
const companies = ref([]);

// Map store IDs coming from the API to existing logo paths
const logoByStoreId = {
  1: '/logo/osherAd.png', // C:\Bennys\SafeCash\KashCashFinder\KashCashFinderFe\logo\osherAd.png
  2: '/logo/mcdonals.png', // C:\Bennys\SafeCash\KashCashFinder\KashCashFinderFe\logo\mcdonals.png
};

const filteredCompanies = computed(() => companies.value);

async function performSearch() {
  try {
    const trimmedQuery = search.value.trim();

    const isEmptyQuery = trimmedQuery.length === 0;

    const url = isEmptyQuery
      ? 'https://localhost:7094/api/v1/search/all?maxResults=200'
      : 'https://localhost:7094/api/v1/search';

    const options =
      isEmptyQuery
        ? {
            method: 'GET',
          }
        : {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify({
              query: trimmedQuery,
              city: '',
              maxResults: 20,
            }),
          };

    const response = await fetch(url, options);

    if (!response.ok) {
      console.error('Search API failed', response.status, response.statusText);
      return;
    }

    const data = await response.json();

    companies.value = (data.results || []).map((store) => ({
      id: store.storeId,
      name: store.name,
      logo: logoByStoreId[store.storeId] || '/logo/osherAd.png',
      tagline: store.address || '',
    }));
  } catch (error) {
    console.error('Error performing search', error);
  }
}

// Create debounced version - waits 500ms after user stops typing before calling API
const debouncedSearch = debounce(performSearch, 2000);

onMounted(() => {
  // Load all companies immediately on page load (no debounce)
  performSearch();
});

watch(search, () => {
  // Use debounced search when user types (waits 500ms after typing stops)
  debouncedSearch();
});
</script>

