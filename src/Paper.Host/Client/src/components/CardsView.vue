<template lang="pug">
  v-layout(justify-center)
    v-flex(
      xs12
    )
      v-container(
        fluid
        grid-list-md
      )
        v-layout(
          row
          wrap
        )
          v-flex(
            xs3
            v-for="item in $paper.cards.items"
            :key="item.properties.title"
          )
            a(@click.stop="openLink(item)")

              v-card
                v-card-title(primary-title)
                  div(class="headline") {{ item.properties.title }}
                  div {{ item.properties.description }}

</template>

<script>
  export default {

    methods: {
      openLink (item) {
        var link = item.links.find(link => link.rel.includes('self'))
        if (link) {
          this.$paper.requester.redirectToPage(link.href)
        }
      }
    }

  }
</script>
