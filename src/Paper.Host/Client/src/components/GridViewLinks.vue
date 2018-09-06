<template lang="pug">

  v-list
    v-list-tile(
      v-for="item in itemLinks(items.index)" 
      :key="item.href"
    )
      v-list-tile-content
        a(
          @click.stop="$paper.requester.redirectToPage(item.href)"
        ) {{ item.title ? item.title : item.rel[0] }}
      

</template>

<script>
  export default {
    props: ['items'],

    methods: {
      itemLinks (index) {
        var entity = this.$paper.getEntity().entities[index]
        var links = []
        if (entity.links && entity.links.length > 0) {
          links = entity.links.filter(link => !link.rel.includes('self'))
        }
        return links
      }
    }
  }
</script>

<style lang="sass">
  .v-list__tile
    height: 32px
</style>