<template lang="pug">
  v-app
    menu-links
    menu-filters
    action-bar
    main-toolbar
    v-content
      router-view(:key="$route.fullPath")
    main-footer
    notifications
</template>

<script>
  import ActionBar from './components/ActionBar.vue'
  import MenuLinks from './components/MenuLinks.vue'
  import MenuFilters from './components/MenuFilters.vue'
  import MainFooter from './components/MainFooter.vue'
  import MainToolbar from './components/MainToolbar.vue'
  export default {
    components: {
      ActionBar,
      MainFooter,
      MainToolbar,
      MenuLinks,
      MenuFilters
    },
    beforeRouteUpdate (to, from, next) {
      next()
    },
    created () {
      this.$http.interceptors.response.use(undefined, (err) => {
        return new Promise((resolve, reject) => {
          if (err.status === 401 && err.config && !err.config.__isRetryRequest) {
            this.$paper.auth.logout
          }
          throw err
        })
      })

      var containsHash = this.$paper.requester.containsHash(window.location.href)
      if (!containsHash) {
        var path = this.$paper.requester.addHashToUrl(window.location.href)
        window.location = path
        return
      }

      this.$paper.load(this.$route)
    }
  }
</script>

<style src="vue-notifyjs/themes/material.css"></style>

<style lang="sass">
  .vue-notifyjs .alert
    z-index: 99999
</style>

