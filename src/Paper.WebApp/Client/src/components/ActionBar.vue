<template lang="pug">
  v-toolbar(
    color="white"
    :class="showClass"
    fixed
  )
    v-btn(
      icon 
      @click="deselected"
    )
      v-icon arrow_back
    
    v-toolbar-title(v-if="!$vuetify.breakpoint.xs")
      v-btn(
        flat
        @click="deselected"
      )
        | Cancelar

    v-spacer
    
    action-bar-form(
      ref="actionBarForm"
    )

    v-subheader
      | {{ selectedItemsLabel }}

    div(v-if="!$vuetify.breakpoint.xs")
      v-btn(
        flat
        v-for="action in actions" 
        :key="action.name"
        @click.stop="actionClick(action)"
      ) {{ $paper.actions.getTitle(action) }}

    v-menu(v-if="$vuetify.breakpoint.xs")
      v-btn(
        icon
        slot="activator"
      )
        v-icon more_vert
      
      v-list
        v-list-tile(
          v-for="action in actions" 
          :key="action.name"
          @click.stop="actionClick(action)"
        )
          v-list-tile-title 
            | {{ $paper.actions.getTitle(action) }}
</template>

<script>
  import { Events } from '../event-bus.js'
  import ActionBarForm from './ActionBarForm.vue'
  export default {
    components: {
      ActionBarForm
    },

    data: () => ({
      actionName: ''
    }),

    computed: {
      showClass () {
        if (!this.$paper.state.selection) {
          return 'display: none'
        }
      },

      actions () {
        var selectedItems = this.$paper.grid.selectedItems
        var actions = this.$paper.actions.getActions(selectedItems)
        return actions && actions.length > 0 ? actions : []
      },

      action () {
        var action = this.actions.filter(a => a.name === this.actionName)[0]
        return action
      },

      selectedItemsLabel () {
        var selectedItems = this.$paper.grid.selectedItems
        var text = selectedItems.length > 1 ? 'itens selecionados' : 'item selecionado'
        return selectedItems.length + ' ' + text
      }
    },

    methods: {
      deselected () {
        Events.$emit('selectState', false)
        this.$paper.state.disableSelectionState()
      },

      actionClick (action) {
        this.openDialog(action)
        this.actionName = action.name
      },

      openDialog (action) {
        this.$refs.actionBarForm.open(action)
      },

      closeDialog () {
        this.$refs.actionBarForm.close()
      }
    }
  }
</script>
