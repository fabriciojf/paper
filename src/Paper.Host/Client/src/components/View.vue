<template lang="pug">
    
  v-container(
    fluid
  )

    p(v-if="$paper.page.hasTitle()")
      div(class="headline") 
        | {{ $paper.page.title }}

    v-flex(
      xs12
      sm8
      offset-xs2
    )

      v-card
        
        v-list

          div(
            v-for="(item, index) in $paper.data.items" 
            :key="item.key"
          )
            v-list-tile(@click="openSelfLink(item)")
              v-list-tile-content(style="width:40%; max-width:40%;")
                v-list-tile-sub-title {{ item.title }}

              v-list-tile-content
                v-list-tile-title(
                  v-if="dataTypeIsBool(item)"
                  class="checked"
                )
                
                v-list-tile-title(v-else)
                  | {{ item.value }}

              v-list-tile-action(
                v-if="hasItemLinks(item)"
                @click.stop=""
              )
                v-menu(
                  offset-y
                  left 
                  bottom
                )
                  span(
                    icon
                    slot="activator"
                    small
                  )
                    v-icon(color="grey")
                      | more_vert

                  v-list
                    v-list-tile(
                      v-for="link in itemLinks(item)" 
                      :key="link.href"
                    )
                      v-list-tile-content
                        a(
                          @click.stop="openLink(link)"
                        ) {{ link.title ? link.title : item.rel[0] }}

            v-divider(v-if="index !== $paper.data.items.length - 1")

</template>

<script>
  export default {
    methods: {
      dataTypeIsBool (item) {
        return item.dataType === this.$paper.dataType.BOOL
      },

      hasItemLinks (item) {
        var itemLinks = this.itemLinks(item)
        return itemLinks && itemLinks.length > 0
      },

      openLink (link) {
        if (link.href) {
          this.$paper.requester.redirectToPage(link.href)
        }
      },

      itemLinks (item) {
        var links = this.$paper.data.getLinks(item.name)
        if (links) {
          return links
        }
      },

      openSelfLink (item) {
        var links = this.itemLinks(item)
        if (links && links.length > 0) {
          var selfLink = links.find(link => link.rel.find(rel => rel === item.name))
          if (selfLink) {
            this.$paper.requester.redirectToPage(selfLink.href)
          }
        }
      }
    }
  }
</script>
