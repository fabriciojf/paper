<template lang="pug">
  v-card
    v-container(
      fluid
      grid-list-md
    )
      div(v-if="$paper.page.hasTitle()")
        div(class="headline") 
          | {{ $paper.page.title }}

      grid-view-pagination

      v-data-table(
        v-model="selected"
        :headers="headers"
        :items="items"
        item-key="_indexRowItemTable"
        hide-actions=true
        :select-all="$paper.grid.hasActions()"
        no-data-text="Não existem dados para exibir"
      )

        template(
          slot="headers" 
          slot-scope="props"
        )
          tr
            th(
              v-for="header in props.headers"
              :key="header.text"
              align="left"
            )

              v-tooltip(bottom)
                span(slot="activator") 
                  v-icon(
                    v-if="header.order === -1"
                    small
                    @click="openPrimaryLink(header.value)"
                  ) arrow_downward
                  v-icon(
                    v-if="header.order === 1"
                    small
                    @click="openPrimaryLink(header.value)"
                  ) arrow_upward
                  span(@click="openPrimaryLink(header.value)") {{ header.text }}

                span {{ $paper.grid.getPrimaryLink(header.value).title }}

              v-menu(
                bottom
                offset-y
                v-if="$paper.grid.hasHeaderLinks(header.value)"
              )
                span(
                  slot="activator"
                  icon
                  small
                  z-index="100000"
                )
                  v-icon(
                    small
                    color="grey"
                  ) more_vert

                v-list
                  v-list-tile(
                    v-for="(item, i) in $paper.grid.getHeaderLinks(header.value)"
                    :key="i"
                    @click="openLink(item.href)"
                  )
                    v-list-tile-title {{ item.title }}

        template(
          slot="items" 
          slot-scope="items"
        )

          tr(
            :style="getRowCursorStyle(items.item)"
            @click.stop="openItemView(items.item)"
          )

            td(v-if="$paper.grid.hasActions()")
              v-checkbox(
                primary
                hide-details
                v-model="items.selected"
                @click.stop="items.selected = !items.selected"
              )
            
            td(
              v-for="(header, index) in headers"
              :key="items.index.toString() + index.toString()"
              :style="getColumnCursorStyle(items.item, header.value)"
              class="text-xs-left"
              nowrap
            ) 
              a(
                v-if="hasColumnItemLink(items.item, header.value)"
                @click.stop="openColumnItemView(items.item, header.value)"
              ) {{ items.item[header.value] }}

              div(v-else) {{ items.item[header.value] }}

            td(
              class="text-xs-center" 
              @click.stop=""
              v-if="hasItemLinks(items.index)"
            )
              v-menu(
                offset-x 
                left 
                bottom 
                v-if="hasItemLinks(items.index)"
              )
                v-btn(
                  icon
                  slot="activator"
                )
                  v-icon
                    | more_vert

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
  import { Events } from '../event-bus.js'
  import GridViewPagination from './GridViewPagination.vue'
  export default {
    components: {
      GridViewPagination
    },

    data: () => ({
      selected: [],
      menuItems: [
        { title: 'Click Me' },
        { title: 'Click Me' },
        { title: 'Click Me' },
        { title: 'Click Me 2' }
      ]
    }),

    created () {
      Events.$on('selectState', this.selectedMode)
    },

    beforeRouteLeave (to, from, next) {
      this.$paper.state.disableSelectionState()
      next()
    },

    methods: {
      getRowCursorStyle (item) {
        var entireItem = this.getRowIndex(item)
        var link = entireItem.getLinkByRel('self')
        var exist = link && link.href && link.href.length > 0
        var cursor = exist ? 'cursor: pointer' : 'cursor: default'
        return cursor
      },

      getColumnCursorStyle (item, column) {
        var entireItem = this.getRowIndex(item)
        var link = entireItem.getLinkByRel(column)
        var exist = link && link.href && link.href.length > 0
        var cursor = exist ? 'cursor: pointer' : ''
        return cursor
      },

      columnKey (column, index, item) {
        return column.value
      },

      selectedMode (selected) {
        if (!selected) {
          this.selected = []
        }
      },

      toggleAll () {
        if (this.selected.length) this.selected = []
        else this.selected = this.items.slice()
      },

      hasColumnItemLink (item, column) {
        var entireItem = this.getRowIndex(item)
        var link = entireItem.getLinkByRel(column)
        return link && link.href && link.href.length > 0
      },

      openColumnItemView (item, column) {
        var entireItem = this.getRowIndex(item)
        var link = entireItem.getLinkByRel(column)
        if (link) {
          this.$paper.requester.redirectToPage(link.href)
        }
      },

      openItemView (item) {
        var entireItem = this.getRowIndex(item)
        var link = entireItem.getLinkByRel('self')
        if (link) {
          this.$paper.requester.redirectToPage(link.href)
        }
      },

      itemLinks (index) {
        var entity = this.$paper.getEntity().entities[index]
        var links = []
        if (entity.links && entity.links.length > 0) {
          links = entity.links.filter(link => !link.rel.includes('self'))
        }
        return links
      },

      hasItemLinks (index) {
        var items = this.itemLinks(index)
        return items && items.length > 0
      },

      getRowIndex (item) {
        return this.$paper.grid.validItems[item._indexRowItemTable]
      },

      openPrimaryLink (headerName) {
        var primaryLink = this.$paper.grid.getPrimaryLink(headerName)
        if (primaryLink) {
          this.$paper.requester.redirectToPage(primaryLink.href)
        }
      },

      openLink (href) {
        if (href) {
          this.$paper.requester.redirectToPage(href)
        }
      }
    },

    computed: {
      items () {
        return this.$paper.grid.items
      },

      headers () {
        return this.$paper.grid.headers
      },

      selectedItems () {
        var selectedItems = []
        this.selected.forEach(item => {
          var itemSelected = this.getRowIndex(item)
          selectedItems.push(itemSelected)
        })
        return selectedItems
      }
    },

    watch: {
      selected () {
        this.$paper.grid.setSelectedItems(this.selectedItems)
      }
    }
  }
</script>
