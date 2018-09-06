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

              v-tooltip(
                bottom
                v-if="$paper.grid.hasPrimaryLink(header.value)"
              )
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
                  span(
                    @click="openPrimaryLink(header.value)"
                    class="pointer"
                  ) {{ header.text }}

                span {{ $paper.grid.getPrimaryLink(header.value).title }}

              span(v-else) {{ header.text }}

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
            th(
              v-if="$paper.actions.hasSubEntitiesActions()"
            )
              span Ações

        template(
          slot="items" 
          slot-scope="items"
        )

          tr(
            :style="getRowCursorStyle(items.item)"
            @click.stop="openItemView(items.item)"
            @contextmenu="showContextMenu(items.item, $event)"
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
              nowrap
            ) 
              v-tooltip(
                bottom
                v-if="hasColumnItemLink(items.item, header.value)"
              )
                a(
                  slot="activator"
                  v-if="hasColumnItemLink(items.item, header.value)"
                  @click.stop="openColumnItemView(items.item, header.value)"
                ) {{ items.item[header.value] }}

                span {{ getColumnLink(items.item, header.value).title }}

              div(
                v-else
                :class="getDataTypeClass(items.item, header)"
              ) {{ isBooleanColumn(header) ? '' : items.item[header.value] }}

            td(
              class="text-xs-center" 
              @click.stop=""
              v-if="hasItemLinks(items.index)"
            )
              v-menu(
                offset-y
                left 
                bottom 
                v-if="hasItemLinks(items.index)"
              )
                span(
                  icon
                  slot="activator"
                  small
                )
                  v-icon
                    | more_vert

                grid-view-links(:items="items")
                      
            td(v-else)

            v-menu(
              v-model="showMenuLinks"
              :position-x="x"
              :position-y="y"
              absolute
              offset-y
              v-if="rowContextMenuVisible(items.index)"
            )
              grid-view-links(:items="items")

</template>

<script>
  import { Events } from '../event-bus.js'
  import GridViewPagination from './GridViewPagination.vue'
  import GridViewLinks from './GridViewLinks.vue'
  export default {
    components: {
      GridViewPagination,
      GridViewLinks
    },

    data: () => ({
      lineSelected: 0,
      selected: [],
      showMenuLinks: false,
      x: 0,
      y: 0
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

      getColumnLink (item, column) {
        var entireItem = this.getRowIndex(item)
        var link = entireItem.getLinkByRel(column)
        return link
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

      rowContextMenuVisible (index) {
        if (index === this.lineSelected) {
          return this.hasItemLinks(index)
        }
        return false
      },

      hasItemLinks (index) {
        var items = this.itemLinks(index)
        var hasLinks = items && items.length > 0
        return hasLinks
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
      },

      getDataTypeClass (item, header) {
        var dataType = this.$paper.grid.getColumnDataType(header.value)
        switch (dataType) {
          case this.$paper.dataType.BOOL:
            var value = item[header.value]
            return value === 1 ? 'checked text-xs-center' : 'text-xs-center'
          default:
            return 'text-xs-left'
        }
      },

      isBooleanColumn (header) {
        var columnType = this.$paper.grid.getColumnDataType(header.value)
        return columnType === this.$paper.dataType.BOOL
      },

      showContextMenu (item, e) {
        this.lineSelected = item._indexRowItemTable
        e.preventDefault()
        this.showMenuLinks = false
        this.x = e.clientX
        this.y = e.clientY
        this.$nextTick(() => {
          this.showMenuLinks = true
        })
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

<style lang="sass">
  .vue-notifyjs .alert
    z-index: 99999

  table.v-table tbody td, table.v-table tbody th
    height: 32px
</style>