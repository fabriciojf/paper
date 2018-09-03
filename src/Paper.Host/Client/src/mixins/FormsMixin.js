import VPaperText from '../components/paper/VPaperText.vue'
import VPaperNumber from '../components/paper/VPaperNumber.vue'
import VPaperNumberInt from '../components/paper/VPaperNumberInt.vue'
import VPaperCheckbox from '../components/paper/VPaperCheckbox.vue'
import VPaperHidden from '../components/paper/VPaperHidden.vue'
import VPaperDate from '../components/paper/VPaperDate.vue'
import VPaperTime from '../components/paper/VPaperTime.vue'
import VPaperSelect from '../components/paper/VPaperSelect.vue'
import VPaperSwitch from '../components/paper/VPaperSwitch.vue'
import VPaperDatetime from '../components/paper/VPaperDatetime.vue'
import VPaperCurrency from '../components/paper/VPaperCurrency.vue'
import VPaperUrl from '../components/paper/VPaperUrl.vue'
import VPaperLabel from '../components/paper/VPaperLabel.vue'
import VPaperDateRange from '../components/paper/VPaperDateRange.vue'

export default {

  components: {
    VPaperText,
    VPaperNumber,
    VPaperNumberInt,
    VPaperCheckbox,
    VPaperHidden,
    VPaperDate,
    VPaperTime,
    VPaperSelect,
    VPaperSwitch,
    VPaperDatetime,
    VPaperCurrency,
    VPaperUrl,
    VPaperDateRange,
    VPaperLabel
  },

  methods: {
    $_formsMixin_dynamicComponent (field) {
      switch (field.type) {
        case this.$paper.type.HIDDEN:
          return 'VPaperHidden'
        case this.$paper.type.DATE:
          return 'VPaperDate'
        case this.$paper.type.TIME:
          return 'VPaperTime'
        case this.$paper.type.URL:
          return 'VPaperUrl'
        case this.$paper.type.TEXT:
          return this._getSubDataType(field.__dataType)
        case this.$paper.type.NUMBER:
          return this._getSubDataType(field.__dataType)
        case this.$paper.type.CHECKBOX:
          return 'VPaperSelect'
        case this.$paper.type.DATETIME:
          return 'VPaperDate'
        default:
          return 'VPaperText'
      }
    },

    $_formsMixin_makeParams (actionName) {
      var params = {}
      var formName = 'form-' + actionName
      var form = this.$refs[formName]
      form.inputs.forEach((field) => {
        var fieldValue = typeof (field.isActive) !== 'undefined' ? field.isActive : field.$attrs.value
        if (fieldValue) {
          var param = field.$attrs.name
          params[param] = fieldValue
        }
      })
      return params
    },

    $_formsMixin_clear (actionName) {
      var formName = 'form-' + actionName
      var form = this.$refs[formName]
      form.reset()
    },

    $_formsMixin_haveInputs (action) {
      var input = action.fields.find(field =>
        field.type !== this.$paper.type.TEXT && field.dataType !== this.$paper.dataType.STRING
      )
      return input !== undefined
    },

    _getSubDataType (dataType) {
      switch (dataType) {
        case this.$paper.dataType.BIT:
        case this.$paper.dataType.BOOL:
        case this.$paper.dataType.BOOLEAN:
          return 'VPaperSwitch'
        case this.$paper.dataType.NUMBER:
        case this.$paper.dataType.INT:
        case this.$paper.dataType.LONG:
          return 'VPaperNumber'
        case this.$paper.dataType.DECIMAL:
        case this.$paper.dataType.DOUBLE:
        case this.$paper.dataType.FLOAT:
          return 'VPaperNumber'
        case this.$paper.dataType.CURRENCY:
          return 'VPaperCurrency'
        case this.$paper.dataType.STRING:
          return 'VPaperLabel'
        default:
          return 'VPaperText'
      }
    }
  }
}
