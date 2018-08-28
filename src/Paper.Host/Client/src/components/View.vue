<template lang="pug">
  v-container(
    fluid
  )
    v-flex(
      xs12
      sm8
      offset-sm2
    )
      v-card(
        class="elevation-3"
      )
        v-card-title(
          primary-title
          v-if="$paper.page.hasTitle()"
        )
          h2 {{ $paper.page.title }}

        v-card-text
          v-container(fluid)
            v-flex(
              xs12 
              sm12
            )
              v-list(two-line)
                v-list-tile(
                  v-for="item in $paper.data.items" 
                  :key="item.key"
                )
                  v-list-tile-content
                    v-list-tile-title
                      | {{ item.value }}
                    v-list-tile-sub-title
                      | {{ item.title }}
</template>

<script>
  export default {
    data: () => ({
      headers: []
    }),

    beforeRouteUpdate (to, from, next) {
      next()
    }
  }
</script>
