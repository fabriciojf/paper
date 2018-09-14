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
            :hidden="item.hidden === 1"
          )
            v-list-tile(
              @click="openSelfLink(item)"
            )
              v-list-tile-content(class="view-title")
                v-list-tile-sub-title {{ item.title }}

              v-list-tile-content(
                :class="detailsClass(item)"
              )
                v-list-tile-title(
                  v-if="dataTypeIsBool(item)"
                  class="checked"
                )
                
                v-list-tile-sub-title(
                  v-else
                  class="text--primary"
                ) {{ shorten(item.value) }}

                  v-paper-visibility-btn(
                    v-if="isLongerText(item.value)"
                    :text="item.value"
                  )
                        
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
  import VPaperVisibilityBtn from './paper/VPaperVisibilityButton.vue'
  export default {
    data: () => ({
      textColumnMaxLength: 80
    }),

    components: {
      VPaperVisibilityBtn
    },

    methods: {
      styleTest (item) {
        var estilo = this.isLongerText(item) ? 'height: 89px' : ''
        return estilo
      },

      detailsClass (item) {
        var detailsClass = this.hasItemLinks(item) ? 'view-details-menu' : 'view-details'
        return detailsClass
      },

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
      },

      shorten (text) {
        if (text && text.length > this.textColumnMaxLength) {
          text = text.substr(0, this.textColumnMaxLength - 3) + '...'
        }
        return text
      },

      isLongerText (text) {
        if (text && text.length > this.textColumnMaxLength) {
          return true
        }
        return false
      }
    }
  }
</script>
