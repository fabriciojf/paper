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

        template(
          slot="items" 
          slot-scope="items"
        )

          tr(
            :style="getRowStyle(items.index)"
            @click.stop="openRowView(items.index)"
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
              :style="getCelStyle(items.index, header.value)"
              nowrap
            ) 

              v-tooltip(
                bottom
                v-if="hasCelLink(items.index, header.value)"
              )
                a(
                  slot="activator"
                  @click.stop="openCelView(items.index, header.value)"
                ) {{ shorten(items.item[header.value]) }}

                span {{ getCelTootip(items.index, header.value) }}

              div(
                v-else
                :class="getDataTypeClass(items.item, header)"
              ) {{ isBooleanColumn(header) ? '' : shorten(items.item[header.value]) }}

                v-paper-visibility-btn(
                  v-if="isLongerText(items.item[header.value])"
                  :text="items.item[header.value]"
                )

            td(
              class="fixed-column" 
              nowrap
              @click.stop=""
              v-if="hasRowLinks(items.index)"
            )
              v-menu(
                offset-y
                left 
                bottom 
                class="menu-actions"
              )
                span(
                  icon
                  slot="activator"
                  style="height: 30px !important"
                  small
                )
                  v-icon(class="menu-actions")
                    | more_vert

                v-list
                  v-list-tile(
                    v-for="link in getRowLinks(items.index)" 
                    :key="link.href"
                  )
                    v-list-tile-content
                      a(
                        @click.stop="redirect(link.href)"
                      ) {{ link.title ? link.title : link.rel[0] }}

</template>

<script>
  import { Events } from '../event-bus.js'
  import GridViewPagination from './GridViewPagination.vue'
  import VPaperVisibilityBtn from './paper/VPaperVisibilityButton.vue'
  export default {
    components: {
      GridViewPagination,
      VPaperVisibilityBtn
    },

    data: () => ({
      selected: [],
      textColumnMaxLength: 50
    }),

    created () {
      Events.$on('selectState', this.selectedMode)
    },

    beforeRouteLeave (to, from, next) {
      this.$paper.state.disableSelectionState()
      next()
    },

    methods: {
      getRowStyle (index) {
        var link = this.$paper.grid.getSelfLinkByRow(index)
        var cursor = link ? 'cursor: pointer' : 'cursor: default'
        return cursor
      },

      getCelStyle (index, column) {
        var links = this.$paper.grid.getLinkByCel(index, column)
        if (links) {
          var cursor = links ? 'cursor: pointer' : ''
          return cursor
        }
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

      hasCelLink (index, column) {
        return this.$paper.grid.hasLinkByCel(index, column)
      },

      openRowView (index) {
        var link = this.$paper.grid.getSelfLinkByRow(index)
        if (link) {
          this.$paper.requester.redirectToPage(link.href)
        }
      },

      openCelView (index, column) {
        var hasCelLink = this.$paper.grid.hasLinkByCel(index, column)
        if (hasCelLink) {
          var link = this.$paper.grid.getLinkByCel(index, column)
          if (link) {
            this.$paper.requester.redirectToPage(link.href)
          }
        }
      },

      getCelTootip (index, column) {
        var hasCelLink = this.$paper.grid.hasLinkByCel(index, column)
        if (hasCelLink) {
          var link = this.$paper.grid.getLinkByCel(index, column)
          if (link) {
            return link.title
          }
        }
      },

      getRowLinks (index) {
        return this.$paper.grid.getRowLinks(index)
      },

      hasRowLinks (index) {
        return this.$paper.grid.hasRowLinks(index)
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

      shorten (text) {
        if (text && text.length > this.textColumnMaxLength) {
          text = text.substr(0, this.textColumnMaxLength - 3) + '...'
        }
        return text
      },

      isLongerText (text) {
        var isLongerText = text && text.length > this.textColumnMaxLength
        return isLongerText
      },

      redirect (href) {
        this.$paper.requester.redirectToPage(href)
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