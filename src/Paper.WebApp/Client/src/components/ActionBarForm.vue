<template lang="pug">
  v-dialog(
    v-model="actionBarForm"
    scrollable
  )
    v-card
      v-card-title(primary-title)
        h2 {{ $paper.actions.getTitle(action) }}

        v-spacer(v-if="$vuetify.breakpoint.xs")
        
        v-menu(
          bottom
          left
          v-if="$vuetify.breakpoint.xs"
        )
          v-btn(
            icon
            slot="activator"
          )
            v-icon more_vert
          
          v-list
            v-list-tile(@click.stop="$_formsMixin_clear(action.name)") 
              | Limpar

            v-list-tile(@click.stop="close") 
              | Fechar
          
      v-card-text
        v-form(
          v-if="action"
          :ref="formName"
        )
          v-layout(
            row 
            wrap 
            v-for="field in fields" 
            :key="field.name"
          )
            v-flex(xs12)
              component(
                :is="$_formsMixin_dynamicComponent(field)"
                :field="field"
              )
      
      v-divider

      v-card-actions
        v-btn(
          color="secondary"
          @click="submit()"
        ) {{ $paper.actions.getTitle(action) }}

        v-btn(
          color="white"
          v-if="!$vuetify.breakpoint.xs"
          @click="$_formsMixin_clear(action.name)"
        ) Limpar

        v-btn(
          color="white"
          v-if="!$vuetify.breakpoint.xs"
          @click.stop="close"
        ) Fechar
</template>

<script>
  import FormsMixin from '../mixins/FormsMixin.js'
  export default {
    data: () => ({
      actionBarForm: false,
      action: null
    }),

    mixins: [
      FormsMixin
    ],

    computed: {
      fields () {
        var selectedItems = this.$paper.grid.selectedItems
        var fields = this.$paper.actions.getActionFields(selectedItems, this.action.name)
        return fields
      },

      formName () {
        return 'form-' + this.action.name
      }
    },

    methods: {
      submit () {
        var queryParams = this.$_formsMixin_makeParams(this.action.name)
        this.$paper.requester.httpRequest(this.action.method, this.action.href, queryParams).then(response => {
          if (response.ok) {
            this.close()
          } else {
            var error = this.$paper.errors.httpTranslate(response.data.status)
            var message = 'Erro ao acessar a url: ' + this.action.href + '. ' + error
            this.$notify({ message: message, type: 'danger' })
          }
        })
      },

      escapeKeyListener (event) {
        if (event.keyCode === 27) {
          this.close()
        }
      },

      open (action) {
        this.action = action
        this.actionBarForm = true
      },

      close () {
        this.actionBarForm = false
      }
    },

    created () {
      document.addEventListener('keyup', this.escapeKeyListener)
    },

    destroyed () {
      document.removeEventListener('keyup', this.escapeKeyListener)
    }
  }
</script>
