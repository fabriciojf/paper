<template lang="pug">
  component(:is='dynamicComponent')
</template>

<script>
  import PaperGrid from './GridView.vue'
  import PaperView from './View.vue'
  export default {
    data: () => ({
      viewShow: ''
    }),

    components: {
      PaperGrid,
      PaperView
    },

    beforeRouteUpdate (to, from, next) {
      if (to.params.length > 0) {
        var path = '/' + to.params.path.join('/')
        this.$paper.setEntityPath(path)
      }
      this.$paper.state.disableSelectionState()
      next()
    },

    created () {
      var path = '/' + this.$route.params.path
      if (this.$route.params.path instanceof Array) {
        path = '/' + this.$route.params.path.join('/')
      }
      this.$paper.setEntityPath(path)
      this.$paper.page.load()
      this.$paper.navigation.setRightMenuVisible(true)
    },

    destroyed () {
      this.$paper.page.unload()
    },

    computed: {
      dynamicComponent () {
        var data = this.$paper.getEntity()
        var isCollection = data && data.class && data.class.find(value =>
          (value === 'list') || (value === 'rows')
        )
        return isCollection ? PaperGrid : PaperView
      }
    }
  }
</script>
