<template lang="pug">
    v-layout(justify-center)
      v-container(
        fluid
        grid-list-lg
      )
        v-layout(
          row
          wrap
        )

          v-flex(
            xs12
            sm6
            md3
            v-for="item in $paper.cards.items"
            :key="item.properties.title"
          )
            a(@click.stop="openLink(item)")

              v-card
                v-layout
                  v-flex(xs2)
                    v-container(
                      fluid
                      style="padding-right:0px"
                    )
                      v-icon(
                        x-large
                        color="secondary"
                      ) {{ item.properties.icon }}
                  v-flex(xs8)
                    v-card-title(primary-title)
                      div(class="headline") {{ item.properties.title }}
                      div {{ item.properties.description }}
                      
                v-divider(light)

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
