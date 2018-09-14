<template lang="pug">
  component(:is='dynamicComponent')
</template>

<script>
  import PaperGrid from './GridView.vue'
  import PaperView from './View.vue'
  import PaperCards from './CardsView.vue'
  export default {
    data: () => ({
      viewShow: ''
    }),

    components: {
      PaperGrid,
      PaperView,
      PaperCards
    },

    beforeRouteUpdate (to, from, next) {
      if (to.params.length > 0) {
        var path = '/' + to.params.path.join('/')
        this.$paper.page.setPagePath(path)
      }
      this.$paper.state.disableSelectionState()
      next()
    },

    created () {
      var path = '/' + this.$route.params.path
      if (this.$route.params.path instanceof Array) {
        path = '/' + this.$route.params.path.join('/')
      }
      this.$paper.page.setPagePath(path)
      this.$paper.page.load()
      this.$paper.navigation.setRightMenuVisible(true)
    },

    destroyed () {
      this.$paper.page.unload()
    },

    computed: {
      dynamicComponent () {
        var pageType = this.$paper.page.type
        switch (pageType) {
          case this.$paper.page.pageType.CARDS:
            return PaperCards
          case this.$paper.page.pageType.GRID:
            return PaperGrid
          default:
            return PaperView
        }
      }
    }
  }
</script>
